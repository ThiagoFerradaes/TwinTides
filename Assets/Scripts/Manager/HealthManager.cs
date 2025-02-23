using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthManager : NetworkBehaviour {

    #region Variables
    [Header("Atributes")]
    [SerializeField] NetworkVariable<float> maxHealth;
    [SerializeField] private float maxShieldAmount;
    [SerializeField] DeathBehaviour deathBehaviour;
    [SerializeField] Material damageMaterial;
    Material originalMaterial;

    // Variaveis do tipo float
    readonly NetworkVariable<float> _currentHealth = new();
    [HideInInspector] public NetworkVariable<float> currentShieldAmount = new();
    readonly NetworkVariable<float> _healMultiply = new(1f);
    readonly NetworkVariable<float> _shieldMultiply = new(1f);
    readonly NetworkVariable<float> _damageMultiply = new(1f);

    // Variaveis do tipo bool
    readonly NetworkVariable<bool> _canBeDamaged = new(true);
    [HideInInspector] public NetworkVariable<bool> isShielded = new(false);
    readonly NetworkVariable<bool> _canBeShielded = new(true);
    readonly NetworkVariable<bool> _canBeHealed = new(true);
    readonly NetworkVariable<bool> _canBeInvulnerable = new(true);
    readonly NetworkVariable<bool> _canReceiveDebuff = new(true);
    readonly NetworkVariable<bool> _canReceiveBuff = new(true);
    readonly NetworkVariable<bool> _isDead = new(false);

    // Dicionarios de Debuffs e buffs
    readonly Dictionary<Type, ActiveDebuff> _listOfActiveDebuffs = new();
    readonly Dictionary<Type, ActiveBuff> _listOfActiveBuffs = new();

    Coroutine _timeToEndShieldCoroutine;


    // Eventos
    public event Action<(float maxHealth, float currentHealth, float currentShield)> UpdateHealth;
    public event Action OnDeath;
    public event Action<Buff, int> OnBuffAdded, OnBuffRemoved;
    public event Action<Debuff, int> OnDebuffAdded, OnDebuffRemoved;
    public event EventHandler OnGeneralDamage;

    // Corrotinas
    Coroutine damageIndicatorCoroutine;
    #endregion

    #region Methods

    #region Unity Lifecycle
    void Start() {
        Inicialize();

        originalMaterial = GetComponent<MeshRenderer>().material;
    }
    void Inicialize() {
        SetMaxHealthServerRpc();
        InvokeUpdateHealth();
    }
    private void OnEnable() {
        _currentHealth.OnValueChanged += UpdateHealthUI;
        currentShieldAmount.OnValueChanged += UpdateHealthUI;
        maxHealth.OnValueChanged += UpdateHealthUI;
    }
    private void OnDisable() {
        _currentHealth.OnValueChanged -= UpdateHealthUI;
        currentShieldAmount.OnValueChanged -= UpdateHealthUI;
        maxHealth.OnValueChanged -= UpdateHealthUI;
    }
    #endregion

    #region Event Handler
    void UpdateHealthUI(float old, float newValue) {
        InvokeUpdateHealth();
    }
    void InvokeUpdateHealth() {
        UpdateHealth?.Invoke((maxHealth.Value, _currentHealth.Value, currentShieldAmount.Value));
    }
    #endregion

    #region HealthManagement
    public float ReturnMaxHealth() {
        return maxHealth.Value;
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetMaxHealthServerRpc() {
        if (!IsServer) return;

        _currentHealth.Value = maxHealth.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ApplyDamageOnServerRPC(float damageTaken, bool hitShield, bool isAfectedByDamageMultiply) {
        if (!_canBeDamaged.Value) { Debug.Log("Can't take Damage" + gameObject.name); return; }
        if (_isDead.Value == true) return;

        if (isShielded.Value && hitShield) {

            if(isAfectedByDamageMultiply) currentShieldAmount.Value -= (damageTaken * _damageMultiply.Value);
            else currentShieldAmount.Value -= damageTaken;

            if (currentShieldAmount.Value <= 0) {
                float overDamage = -currentShieldAmount.Value;
                currentShieldAmount.Value = 0f;
                isShielded.Value = false;
                _currentHealth.Value = Mathf.Clamp((_currentHealth.Value - overDamage), 0, maxHealth.Value);
            }
        }
        else {

            if(isAfectedByDamageMultiply)
            _currentHealth.Value = Mathf.Clamp((_currentHealth.Value - damageTaken * _damageMultiply.Value), 0, maxHealth.Value);

            else _currentHealth.Value = Mathf.Clamp((_currentHealth.Value - damageTaken), 0, maxHealth.Value);
        }
        if (_currentHealth.Value <= 0) {
            _isDead.Value = true;
            DeathHandlerRpc();
        }
        else {
            OnGeneralDamage?.Invoke(this, EventArgs.Empty);
            TookDamage();
        }
    }

    void TookDamage() {
        DamageIndicatorRpc();    
    }
    [Rpc(SendTo.ClientsAndHost)]
    void DamageIndicatorRpc() {
        if (damageIndicatorCoroutine != null) {
            return;
        }
        damageIndicatorCoroutine = StartCoroutine(DamageIndicator());
    }
    IEnumerator DamageIndicator() {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        
        for (int i = 0; i < 3; i++) {
            mesh.material = damageMaterial;
            yield return new WaitForSeconds(0.06f);
            mesh.material = originalMaterial;
            yield return new WaitForSeconds(0.06f);
        }

        yield return new WaitForSeconds(0.1f);
        damageIndicatorCoroutine = null;
    }

    [Rpc(SendTo.ClientsAndHost)]
    void DeathHandlerRpc() {
        if (damageIndicatorCoroutine != null) StopCoroutine(damageIndicatorCoroutine);

        OnDeath?.Invoke();

        deathBehaviour.Death(this.gameObject);   
    }
    public void ReviveHandler(float percentOfMaxHealth) {

        _currentHealth.Value = Mathf.Clamp((percentOfMaxHealth / 100 * maxHealth.Value), 0.2f * maxHealth.Value, maxHealth.Value);

        _canBeDamaged.Value = true;

        currentShieldAmount.Value = 0f;

        isShielded.Value = false;

        GetComponent<MeshRenderer>().material = originalMaterial;

        ReviveRpc();
    }
    [Rpc(SendTo.Server)]
    void ReviveRpc() {
        _isDead.Value = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void HealServerRpc(float healAmount) {
        if (!_canBeHealed.Value) return;
        if (!IsServer) return;
        _currentHealth.Value = Mathf.Clamp((_currentHealth.Value + healAmount * _healMultiply.Value), 0, maxHealth.Value);
    }

    public float ReturnCurrentHealth() {
        return _currentHealth.Value;
    }
    public bool ReturnDeathState() {
        return _isDead.Value;
    }
    #endregion

    #region ShieldManagement

    [ServerRpc(RequireOwnership = false)]
    public void ApplyShieldServerRpc(float shieldAmount, float durationOfShield, bool isCumulative) {

        if (!_canBeShielded.Value) return; // não pode receber escudo

        if (IsServer) {
            if (isCumulative) currentShieldAmount.Value = Mathf.Clamp((currentShieldAmount.Value + shieldAmount * _shieldMultiply.Value)
            , 0, maxShieldAmount); // o escudo recebido acumula com o escudo atual

            // o escudo recebido não acumula com o escudo atual
            else currentShieldAmount.Value = Mathf.Clamp(shieldAmount * _shieldMultiply.Value, 0, maxShieldAmount);

            isShielded.Value = currentShieldAmount.Value > 0;

            if (_timeToEndShieldCoroutine != null) StopCoroutine(_timeToEndShieldCoroutine);
            _timeToEndShieldCoroutine = StartCoroutine(RemoveShieldAfterDuration(durationOfShield));
        }
    }
    [Rpc(SendTo.Server)]
    public void BreakShieldRpc() {
        currentShieldAmount.Value = 0;
        isShielded.Value = false;
    }
    IEnumerator RemoveShieldAfterDuration(float time) {
        yield return new WaitForSeconds(time);
        currentShieldAmount.Value = 0;
        isShielded.Value = false;
    }
    public bool ReturnShieldStatus() {
        return isShielded.Value;
    }
    #endregion

    #region Debuff Manager
    public void AddDebuffToList(HealthDebuff debuff) {
        if (!_canReceiveDebuff.Value) return; // não pode receber debuffs

        // verificação se ja existe o debuff
        if (_listOfActiveDebuffs.TryGetValue(debuff.GetType(), out ActiveDebuff currentDebuff)) {

            // Registrando quantos stacks tem
            if (currentDebuff.Stack <= debuff.MaxAmountOfStacks) {
                currentDebuff.Stack = Mathf.Min(currentDebuff.Stack + debuff.AddStacks, debuff.MaxAmountOfStacks);
            }

            // paramos a corrotina do debuff
            if (currentDebuff.Coroutine != null) {
                StopCoroutine(currentDebuff.Coroutine);
                currentDebuff.Coroutine = null;

                // Parando o efeito de debuff
                currentDebuff.Debuff.StopDebuff(this);
            }

            // alteramos o dicionario
            currentDebuff.Coroutine =  debuff.ApplyDebuff(this, currentDebuff.Stack);
            _listOfActiveDebuffs.Add(debuff.GetType(), currentDebuff);

            StartCoroutine(currentDebuff.Coroutine); // começamos a corrotina novamente
        }

        else {
            // criamos um tuple com o stack inicial e a corrotina
            ActiveDebuff newDebuff = new(debuff, debuff.InicialStack, debuff.ApplyDebuff(this, debuff.InicialStack));
            _listOfActiveDebuffs.Add(debuff.GetType(), newDebuff); // adicionamos ao dicionario

            StartCoroutine(newDebuff.Coroutine); // começamos a corrotina 
            Debug.Log("Debuff added: " + debuff.name);
        }

        OnDebuffAdded?.Invoke(debuff, _listOfActiveDebuffs[debuff.GetType()].Stack);
    }
    public void RemoveDebuff(HealthDebuff debuff) {
        if (_listOfActiveDebuffs.TryGetValue(debuff.GetType(), out ActiveDebuff currentDebuff)) {
            if (currentDebuff.Coroutine != null) { StopCoroutine(currentDebuff.Coroutine); currentDebuff.Coroutine = null; }
            _listOfActiveDebuffs.Remove(debuff.GetType());
        }
        Debug.Log("Debuff removed: " + debuff.name);
        OnDebuffRemoved?.Invoke(debuff, 0);
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void CleanAllDebuffsRpc() {
        foreach (var debuff in _listOfActiveDebuffs.Values) {
            if (debuff.Coroutine != null) {
                StopCoroutine(debuff.Coroutine);
                debuff.Coroutine = null;
                debuff.Debuff.StopDebuff(this);
            }
        }
        _listOfActiveBuffs.Clear();
    }
    #endregion

    #region Buff Manager
    public void AddBuffToList(HealthBuff buff) {
        if (!_canReceiveBuff.Value) return; // não pode receber buff

        // verificação se ja existe o buff
        if (_listOfActiveBuffs.TryGetValue(buff.GetType(), out ActiveBuff currentBuff)) {

            // Registrando quantos stacks tem
            if (currentBuff.Stack <= buff.MaxAmountOfStacks) {
                currentBuff.Stack = Mathf.Min(currentBuff.Stack + buff.AddStacks, buff.MaxAmountOfStacks);
            }

            // paramos a corrotina do Buff
            if (currentBuff.Coroutine != null) {
                StopCoroutine(currentBuff.Coroutine);
                currentBuff.Coroutine = null;

                // Parando o efeito do buff
                currentBuff.Buff.StopBuff(this);
            }

            // alteramos o dicionario
            currentBuff.Coroutine = buff.ApplyBuff(this, currentBuff.Stack);
            _listOfActiveBuffs.Add(buff.GetType(), currentBuff);

            StartCoroutine(currentBuff.Coroutine); // começamos a corrotina novamente
        }

        else {
            // criamos um novo buff com o stack inicial e a corrotina
            ActiveBuff newBuff = new(buff, buff.InicialStack, buff.ApplyBuff(this, buff.InicialStack));
            _listOfActiveBuffs.Add(buff.GetType(), newBuff); // adicionamos ao dicionario

            StartCoroutine(newBuff.Coroutine); // começamos a corrotina 
            Debug.Log("Buff added: " + buff.name);
        }

        OnBuffAdded?.Invoke(buff, _listOfActiveBuffs[buff.GetType()].Stack);
    }
    public void RemoveBuff(HealthBuff buff) {
        if (_listOfActiveBuffs.TryGetValue(buff.GetType(), out ActiveBuff currentBuff)) {
            if (currentBuff.Coroutine != null) { StopCoroutine(currentBuff.Coroutine); currentBuff.Coroutine = null; }
            _listOfActiveBuffs.Remove(buff.GetType());
        }
        Debug.Log("Buff removed: " + buff.name);
        OnBuffRemoved?.Invoke(buff, 0);
    }
    public void CleanAllBuffs() {
        foreach (var buff in _listOfActiveBuffs.Values) {
            if (buff.Coroutine != null) {
                StopCoroutine(buff.Coroutine);
                buff.Coroutine = null;
                buff.Buff.StopBuff(this);
            }
        }
        _listOfActiveBuffs.Clear();
    }
    #endregion

    #region Variables Management
    [ServerRpc]
    public void SetPermissionServerRpc(HealthPermissions permission ,bool state) {
        if (!IsServer) return;

        switch (permission) {
            case HealthPermissions.CanBeHealed:
                _canBeHealed.Value = state;
                break;
            case HealthPermissions.CanBeShielded:
                _canBeShielded.Value = state;
                Debug.Log(gameObject.name + " Can be shielded: " + _canBeShielded.Value);
                break;
            case HealthPermissions.CanTakeDamage: // fica invulneravel
                if (_canBeInvulnerable.Value) _canBeDamaged.Value = state;
                break;
            case HealthPermissions.CanBeInvulnerable:
                _canBeInvulnerable.Value = state;
                break;
            case HealthPermissions.CanBeDebuffed:
                _canReceiveDebuff.Value = state;
                break;
            case HealthPermissions.CanBeBuffed:
                _canReceiveBuff.Value = state;
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeMaxHealthServerRpc(float newMaxHealthValue) {
        if (!IsServer) return;

        float percentageOfCurrentHealth = _currentHealth.Value / maxHealth.Value;
        maxHealth.Value = newMaxHealthValue;
        _currentHealth.Value = Mathf.Min(percentageOfCurrentHealth * maxHealth.Value, maxHealth.Value);

    }

    [ServerRpc(RequireOwnership = false)]
    public void SetMultiplyServerRpc(HealthMultipliers multiplier ,float newHealMultiply) {
        if (!IsServer) return;

        switch (multiplier) {
            case HealthMultipliers.Heal:
                _healMultiply.Value *= Mathf.Clamp(newHealMultiply, 0, 2);
                Debug.Log(gameObject.name + " Heal Multiply: " + _healMultiply.Value);
                break;
            case HealthMultipliers.Shield:
                _shieldMultiply.Value *= Mathf.Clamp(newHealMultiply, 0, 2);
                break;
            case HealthMultipliers.Damage:
                _damageMultiply.Value *= Mathf.Clamp(newHealMultiply, 0, 2);
                break;
        }
    }
    #endregion


    #endregion
}

#region Classes Debuff e Buff
public class ActiveDebuff {
    public HealthDebuff Debuff { get; }
    public int Stack { get; set; }
    public IEnumerator Coroutine { get; set; }

    public ActiveDebuff(HealthDebuff debuff, int stack, IEnumerator coroutine) {
        Debuff = debuff;
        Stack = stack;
        Coroutine = coroutine;
    }
}

public class ActiveBuff {
    public HealthBuff Buff { get; }
    public int Stack { get; set; }
    public IEnumerator Coroutine { get; set; }

    public ActiveBuff(HealthBuff buff, int stack, IEnumerator coroutine) {
        Buff = buff;
        Stack = stack;
        Coroutine = coroutine;
    }
}
#endregion
