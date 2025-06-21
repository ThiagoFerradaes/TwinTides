using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class HealthManager : NetworkBehaviour {

    #region Variables
    [Header("Atributes")]
    [SerializeField] float health;
    [SerializeField] private float maxShieldAmount;
    [SerializeField] DeathBehaviour deathBehaviour;
    [SerializeField] Material damageMaterial;
    [SerializeField] EventReference RecieveHealSound;
    [SerializeField] EventReference RecieveDamageSound;
    [SerializeField] EventReference RecieveShieldSound;
    [SerializeField] float healIndicatorCooldown;
    [SerializeField] float shieldIndicatorCooldown;
    [SerializeField] float timeBetweenDamageIndicators;
    Material originalMaterial;

    // Network

    // vida
    private NetworkVariable<float> _maxHealth = new();
    private NetworkVariable<float> _currentHealth = new();

    // Multiplicadores
    private NetworkVariable<float> _healMultiply = new(1f);
    private NetworkVariable<float> _shieldMultiply = new(1f);
    private NetworkVariable<float> _damageMultiply = new(1f);

    // escudo
    private NetworkVariable<float> _currentShieldAmount = new();

    // Permissões
    private NetworkVariable<bool> _canBeDamaged = new(true);
    private NetworkVariable<bool> _isShielded = new(false);
    private NetworkVariable<bool> _canBeShielded = new(true);
    private NetworkVariable<bool> _canBeHealed = new(true);
    private NetworkVariable<bool> _canBeInvulnerable = new(true);
    private NetworkVariable<bool> _canReceiveDebuff = new(true);
    private NetworkVariable<bool> _canReceiveBuff = new(true);

    // Morte
    private NetworkVariable<bool> _isDead = new(false);


    // Dicionarios de Debuffs e buffs
    readonly Dictionary<Type, ActiveDebuff> _listOfActiveDebuffs = new();
    readonly Dictionary<Type, ActiveBuff> _listOfActiveBuffs = new();

    // Corrotina
    Coroutine _timeToEndShieldCoroutine;
    Coroutine damageIndicatorCoroutine;
    Coroutine healIndicatorCoroutine;
    Coroutine shieldIndicatorCoroutine;

    // Eventos
    public event Action<(float maxHealth, float currentHealth, float currentShield, float maxShield)> OnHealthUpdate;
    public event Action OnDeath;
    public event Action OnRevive;
    public event Action<Buff, int> OnBuffAdded, OnBuffRemoved;
    public event Action<Debuff, int> OnDebuffAdded, OnDebuffRemoved;
    public static event System.EventHandler OnMelHealed;

    #endregion

    #region Methods

    #region Initialize
    public override void OnNetworkSpawn() {
        Initialize();

        originalMaterial = GetComponent<MeshRenderer>().material;
    }
    void Initialize() {
        if (IsServer) {
            _maxHealth.Value = health; // Igualando a vida máxima a vida disponível no inspecto
            _currentHealth.Value = _maxHealth.Value; // Igualando a vida atual a vida máxima
        }

        UpdateHealthUI(0, 0);
    }
    private void OnEnable() {
        _currentHealth.OnValueChanged += UpdateHealthUI;
        _currentShieldAmount.OnValueChanged += UpdateHealthUI;
        _maxHealth.OnValueChanged += UpdateHealthUI;
    }
    private void OnDisable() {
        _currentHealth.OnValueChanged -= UpdateHealthUI;
        _currentShieldAmount.OnValueChanged -= UpdateHealthUI;
        _maxHealth.OnValueChanged -= UpdateHealthUI;
    }
    void UpdateHealthUI(float old, float newValue) {
        OnHealthUpdate?.Invoke((_maxHealth.Value, _currentHealth.Value, _currentShieldAmount.Value, maxShieldAmount));
    }
    #endregion

    #region HealthManagement

    // Getters 
    public float ReturnMaxHealth() => _maxHealth.Value;
    public float ReturnCurrentHealth() => _currentHealth.Value;
    public bool ReturnDeathState() => _isDead.Value;


    // Damage
    public void DealDamage(float damageTaken, bool hitShield, bool isAffectedByDamageMultiply) {
        if (!IsServer || !_canBeDamaged.Value || _isDead.Value) return;

        float finalDamage = isAffectedByDamageMultiply ? damageTaken * _damageMultiply.Value : damageTaken;

        if (_isShielded.Value && hitShield) {
            ApplyShieldDamage(finalDamage);
        }
        else {
            ApplyHealthDamage(finalDamage);
        }

        if (_currentHealth.Value <= 0) {
            _isDead.Value = true;
            HandleDeathRpc();
        }
        else {
            DamageIndicatorRpc();
        }
    }

    private void ApplyShieldDamage(float damage) {
        _currentShieldAmount.Value -= damage;

        if (_currentShieldAmount.Value <= 0f) {
            float overDamage = -_currentShieldAmount.Value;
            _currentShieldAmount.Value = 0f;
            _isShielded.Value = false;

            ApplyHealthDamage(overDamage);
        }
    }

    private void ApplyHealthDamage(float damage) {
        _currentHealth.Value = Mathf.Clamp(_currentHealth.Value - damage, 0, _maxHealth.Value);
    }


    // Death Handling
    public void Kill() {
        if (IsServer) {
            _isDead.Value = true;
            HandleDeathRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void HandleDeathRpc() {
        if (damageIndicatorCoroutine != null)
            StopCoroutine(damageIndicatorCoroutine);

        StopAllCoroutines();

        _listOfActiveBuffs.Clear();
        _listOfActiveDebuffs.Clear();

        OnDeath?.Invoke();
        deathBehaviour.Death(this.gameObject);
    }

    // Heal
    public void Heal(float healAmount, bool melHealed) {
        bool isFullLife = _currentHealth.Value >= _maxHealth.Value;

        if (!_canBeHealed.Value || isFullLife) return;

        if (melHealed) OnMelHealed?.Invoke(this, EventArgs.Empty);

        if (!IsServer) return;

        StartHealIndicatorRpc();
        _currentHealth.Value = Mathf.Clamp((_currentHealth.Value + healAmount * _healMultiply.Value), 0, _maxHealth.Value);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void StartHealIndicatorRpc() {
        healIndicatorCoroutine ??= StartCoroutine(HealIndicatorRoutine());
    }

    IEnumerator HealIndicatorRoutine() {

        if (!RecieveHealSound.IsNull) RuntimeManager.PlayOneShot(RecieveHealSound, transform.position);

        yield return new WaitForSeconds(healIndicatorCooldown);

        healIndicatorCoroutine = null;
    }

    // Revive
    public void ReviveHandler(float percentOfMaxHealth) {

        if (IsServer) {
            _currentHealth.Value = Mathf.Clamp((percentOfMaxHealth / 100 * _maxHealth.Value), 0.2f * _maxHealth.Value, _maxHealth.Value);

            _canBeDamaged.Value = true;

            _isShielded.Value = false;

            _isDead.Value = false;

            _currentShieldAmount.Value = 0f;

        }
        GetComponent<MovementManager>().UnStun();

        GetComponent<MeshRenderer>().material = originalMaterial;

        ReviveAnimation();

        OnRevive?.Invoke();

    }

    void ReviveAnimation() {
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null) {
            bool hasTrigger = false;

            foreach (var param in anim.parameters) {
                if (param.name == "Reviveu" && param.type == AnimatorControllerParameterType.Trigger) {
                    hasTrigger = true;
                    break;
                }
            }

            if (hasTrigger) {
                anim.SetTrigger("Reviveu");
            }
            else {
                Debug.LogWarning("Trigger 'Reviveu' não existe no Animator Controller.");
            }
        }

    }

    // Visual
    [Rpc(SendTo.ClientsAndHost)]
    void DamageIndicatorRpc() {
        if (damageIndicatorCoroutine == null && gameObject.activeInHierarchy)
            damageIndicatorCoroutine = StartCoroutine(DamageIndicator());
    }
    IEnumerator DamageIndicator() {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (!RecieveDamageSound.IsNull) RuntimeManager.PlayOneShot(RecieveDamageSound, transform.position);

        for (int i = 0; i < 3; i++) {
            mesh.material = damageMaterial;
            yield return new WaitForSeconds(timeBetweenDamageIndicators);
            mesh.material = originalMaterial;
            yield return new WaitForSeconds(timeBetweenDamageIndicators);
        }

        yield return new WaitForSeconds(0.1f);
        damageIndicatorCoroutine = null;
    }


    #endregion

    #region ShieldManagement

    // Getters
    public bool ReturnShieldStatus() => _isShielded.Value;
    public float ReturnShieldAmount() => _currentShieldAmount.Value;

    public float ReturnMaxShieldAmount() => maxShieldAmount;

    // Shield 
    public void ApplyShield(float shieldAmount, float durationOfShield, bool isCumulative) {
        if (!IsServer || !_canBeShielded.Value) return;

        float calculatedShield = shieldAmount * _shieldMultiply.Value;

        _currentShieldAmount.Value = isCumulative ? Mathf.Clamp(_currentShieldAmount.Value + calculatedShield, 0, maxShieldAmount) : Mathf.Clamp(calculatedShield, 0, maxShieldAmount);

        _isShielded.Value = _currentShieldAmount.Value > 0;

        if (_timeToEndShieldCoroutine != null) StopCoroutine(_timeToEndShieldCoroutine);

        if (gameObject.activeInHierarchy) {
            _timeToEndShieldCoroutine = StartCoroutine(RemoveShieldAfterDuration(durationOfShield));
            StartShieldIndicatorCoroutineRpc();
        }
    }
    public void BreakShield() {
        if (!IsServer) return;

        _currentShieldAmount.Value = 0;
        _isShielded.Value = false;
    }
    IEnumerator RemoveShieldAfterDuration(float time) {
        yield return new WaitForSeconds(time);
        _currentShieldAmount.Value = 0;
        _isShielded.Value = false;
    }

    [Rpc(SendTo.ClientsAndHost)]
    void StartShieldIndicatorCoroutineRpc() {
        shieldIndicatorCoroutine ??= StartCoroutine(ShieldIndicator());
    }
    IEnumerator ShieldIndicator() {
        if (!RecieveShieldSound.IsNull) RuntimeManager.PlayOneShot(RecieveShieldSound, transform.position);

        yield return new WaitForSeconds(shieldIndicatorCooldown);

        shieldIndicatorCoroutine = null;
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
            currentDebuff.Coroutine = debuff.ApplyDebuff(this, currentDebuff.Stack);
            _listOfActiveDebuffs.Add(debuff.GetType(), currentDebuff);

            if (gameObject.activeInHierarchy) StartCoroutine(currentDebuff.Coroutine); // começamos a corrotina novamente
        }

        else {
            // criamos um tuple com o stack inicial e a corrotina
            ActiveDebuff newDebuff = new(debuff, debuff.InicialStack, debuff.ApplyDebuff(this, debuff.InicialStack));
            _listOfActiveDebuffs.Add(debuff.GetType(), newDebuff); // adicionamos ao dicionario

            if (gameObject.activeInHierarchy) StartCoroutine(newDebuff.Coroutine); // começamos a corrotina 
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

    public void CleanAllDebuffs() {
        if (!IsServer) return;

        CleanAllDebuffsRpc();
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void CleanAllDebuffsRpc() {
        var debuffsToClean = _listOfActiveDebuffs.Values.ToList();

        foreach (var debuff in debuffsToClean) {
            if (debuff.Coroutine != null) {
                StopCoroutine(debuff.Coroutine);
                debuff.Coroutine = null;
                debuff.Debuff.StopDebuff(this);
            }
        }

        _listOfActiveDebuffs.Clear();
    }

    public bool CheckIfHasDebuff(HealthDebuff debuff) {
        return _listOfActiveDebuffs.TryGetValue(debuff.GetType(), out ActiveDebuff currentDebuff);
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

            if (gameObject.activeInHierarchy) StartCoroutine(currentBuff.Coroutine); // começamos a corrotina novamente
        }

        else {
            // criamos um novo buff com o stack inicial e a corrotina
            ActiveBuff newBuff = new(buff, buff.InicialStack, buff.ApplyBuff(this, buff.InicialStack));
            _listOfActiveBuffs.Add(buff.GetType(), newBuff); // adicionamos ao dicionario

            if (gameObject.activeInHierarchy) StartCoroutine(newBuff.Coroutine); // começamos a corrotina 
        }

        OnBuffAdded?.Invoke(buff, _listOfActiveBuffs[buff.GetType()].Stack);
    }
    public void RemoveBuff(HealthBuff buff) {
        if (_listOfActiveBuffs.TryGetValue(buff.GetType(), out ActiveBuff currentBuff)) {
            if (currentBuff.Coroutine != null) { StopCoroutine(currentBuff.Coroutine); currentBuff.Coroutine = null; }
            _listOfActiveBuffs.Remove(buff.GetType());
        }
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
    [ServerRpc(RequireOwnership = false)]
    public void SetPermissionServerRpc(HealthPermissions permission, bool state) {
        if (!IsServer) return;

        switch (permission) {
            case HealthPermissions.CanBeHealed:
                _canBeHealed.Value = state;
                break;
            case HealthPermissions.CanBeShielded:
                _canBeShielded.Value = state;
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

        float percentageOfCurrentHealth = _currentHealth.Value / _maxHealth.Value;
        _maxHealth.Value = newMaxHealthValue;
        _currentHealth.Value = Mathf.Min(percentageOfCurrentHealth * _maxHealth.Value, _maxHealth.Value);

    }

    [ServerRpc(RequireOwnership = false)]
    public void SetMultiplyServerRpc(HealthMultipliers multiplier, float newHealMultiply) {
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

    [Rpc(SendTo.Server)]
    public void InvulnerabilityRpc(bool on) {
        if (on) {
            _canBeDamaged.Value = false;
            _canReceiveDebuff.Value = false;
        }
        else {
            _canBeDamaged.Value = true;
            _canReceiveDebuff.Value = true;
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

