using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class HealthManager : NetworkBehaviour {

    #region Variables
    [Header("Atributes")]
    public NetworkVariable<float> maxHealth;
    [SerializeField] private float maxShieldAmount;

    // Variaveis do tipo float
    readonly NetworkVariable<float> _currentHealth = new();
    [HideInInspector] public NetworkVariable<float> currentShieldAmount = new();
    readonly NetworkVariable<float> _healMultiply = new(1f);
    readonly NetworkVariable<float> _shieldMultiply = new(1f);
    readonly NetworkVariable<float> _damageMultiply = new(1f);

    // Variaveis do tipo bool
    readonly NetworkVariable<bool> _canTakeDamage = new(true);
    [HideInInspector] public NetworkVariable<bool> isShielded = new(false);
    readonly NetworkVariable<bool> _canReceiveShield = new(true);
    readonly NetworkVariable<bool> _canBeHealed = new(true);
    readonly NetworkVariable<bool> _canBeInvulnerable = new(true);

    // Lista de debuffs atuais
    readonly Dictionary<Type, (int, IEnumerator)> _listOfActiveDebuffs = new();

    Coroutine _timeToEndShieldCoroutine;
    Coroutine _timeToEndInvulnerabilityCoroutine;

    // Eventos
    public event Action<(float maxHealth, float currentHealth, float currentShield)> UpdateHealth;
    public event Action OnDeath;
    #endregion

    #region Methods
    void Start() {
        _currentHealth.OnValueChanged += UpdateHealthUI;
        currentShieldAmount.OnValueChanged += UpdateHealthUI;
        maxHealth.OnValueChanged += UpdateHealthUI;
        SetMaxHealthServerRpc();
        UpdateHealth?.Invoke((maxHealth.Value, _currentHealth.Value, currentShieldAmount.Value));
    }
    private void OnEnable() {
        
    }
    private void OnDisable() {
        _currentHealth.OnValueChanged -= UpdateHealthUI;
        currentShieldAmount.OnValueChanged -= UpdateHealthUI;
        maxHealth.OnValueChanged -= UpdateHealthUI;
    }
    void UpdateHealthUI(float old, float newValue) {
        UpdateHealth?.Invoke((maxHealth.Value, _currentHealth.Value, currentShieldAmount.Value));
    }
    public void AddDebuffToList(HealthDebuff debuff) {
        //if (!IsServer) return;
        if (_listOfActiveDebuffs.ContainsKey(debuff.GetType())) {
            StopCoroutine(_listOfActiveDebuffs[debuff.GetType()].Item2); // paramos a corrotina do debuff
            var stacks = _listOfActiveDebuffs[debuff.GetType()].Item1;

            if (stacks <= debuff.MaxAmountOfStacks) {
                stacks = Mathf.Min(stacks + debuff.AddStacks, debuff.MaxAmountOfStacks);
            }

            // alteramos o dicionario
            _listOfActiveDebuffs[debuff.GetType()] = (stacks, debuff.ApplyDebuff(this, stacks));

            StartCoroutine(_listOfActiveDebuffs[debuff.GetType()].Item2); // começamos a corrotina novamente
        }

        else {
            // criamos um tuple com o stack inicial e a corrotina
            var tuple = (debuff.InicialStack, debuff.ApplyDebuff(this, debuff.InicialStack));
            _listOfActiveDebuffs.Add(debuff.GetType(), tuple); // adicionamos ao dicionario

            StartCoroutine(_listOfActiveDebuffs[debuff.GetType()].Item2); // começamos a corrotina 
        }
    }
    public void RemoveDebuff(HealthDebuff debuff) {
        if (!_listOfActiveDebuffs.ContainsKey(debuff.GetType())) return;

        StopCoroutine(_listOfActiveDebuffs[debuff.GetType()].Item2);

        _listOfActiveDebuffs.Remove(debuff.GetType());
    }

    [ServerRpc(RequireOwnership = false)]
    public void ApplyDamageOnServerRPC(float damage, bool hitShield) {
        TakeDamage(damage, hitShield);
    }
    void TakeDamage(float damageTaken, bool hitShield) {
        if (!IsServer) return;
        if (!_canTakeDamage.Value) { Debug.Log("Can't take Damage" + gameObject.name); return; }

        if (isShielded.Value && hitShield) {
            currentShieldAmount.Value -= (damageTaken * _damageMultiply.Value);

            if (currentShieldAmount.Value <= 0) {
                float overDamage = -currentShieldAmount.Value;
                currentShieldAmount.Value = 0f;
                isShielded.Value = false;
                _currentHealth.Value = Mathf.Clamp((_currentHealth.Value - overDamage), 0, maxHealth.Value);
            }
        }
        else {
            _currentHealth.Value = Mathf.Clamp((_currentHealth.Value - damageTaken * _damageMultiply.Value), 0, maxHealth.Value);
        }
        if (_currentHealth.Value <= 0) {
            Death();
        }
    }

    void Death() {
        OnDeath?.Invoke();
        Debug.Log("Is dead: " + gameObject.name);
    }
    public void Revive(float percentOfMaxHealth) {
        _currentHealth.Value = Mathf.Clamp((percentOfMaxHealth / 100 * maxHealth.Value), 0.2f * maxHealth.Value, maxHealth.Value);
        _canTakeDamage.Value = true;
        currentShieldAmount.Value = 0f;
        isShielded.Value = false;
    }
    public void SetInvulnerability(bool state, float duration) {
        if (!_canBeInvulnerable.Value) return;

        _canTakeDamage.Value = state;

        if (_canTakeDamage.Value && _timeToEndInvulnerabilityCoroutine != null) {
            StopCoroutine(_timeToEndInvulnerabilityCoroutine);
            _timeToEndInvulnerabilityCoroutine = StartCoroutine(TimeToEndInvulnerability(duration));
        }
    }
    IEnumerator TimeToEndInvulnerability(float time) {
        yield return new WaitForSeconds(time);
        _canTakeDamage.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReceiveShieldServerRpc(float shieldAmount, float durationOfShield, bool isCumulative) {

        if (!_canReceiveShield.Value) return; // não pode receber escudo

        if (IsServer) {
            if (isCumulative) currentShieldAmount.Value = Mathf.Clamp((currentShieldAmount.Value + shieldAmount * _shieldMultiply.Value)
            , 0, maxShieldAmount); // o escudo recebido acumula com o escudo atual

            // o escudo recebido não acumula com o escudo atual
            else currentShieldAmount.Value = Mathf.Clamp(shieldAmount * _shieldMultiply.Value, 0, maxShieldAmount);

            isShielded.Value = currentShieldAmount.Value > 0;

            if (_timeToEndShieldCoroutine != null) StopCoroutine(_timeToEndShieldCoroutine);
            _timeToEndShieldCoroutine = StartCoroutine(TimeToEndShield(durationOfShield));
        }
    }
    IEnumerator TimeToEndShield(float time) {
        yield return new WaitForSeconds(time);
        currentShieldAmount.Value = 0;
        isShielded.Value = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void HealServerRpc(float healAmount) {
        if (!_canBeHealed.Value) return;
        if (!IsServer) return;
        _currentHealth.Value = Mathf.Clamp((_currentHealth.Value + healAmount * _healMultiply.Value), 0, maxHealth.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeMaxHealthServerRpc(float newMaxHealthValue) {
        if (IsServer) {
            float percentageOfCurrentHealth = _currentHealth.Value / maxHealth.Value;
            maxHealth.Value = newMaxHealthValue;
            _currentHealth.Value = Mathf.Min(percentageOfCurrentHealth * maxHealth.Value, maxHealth.Value);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetMaxHealthServerRpc() {
        if (IsServer) {
            _currentHealth.Value = maxHealth.Value;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetHealMultiplyServerRpc(float newHealMultiply) {
        if (!IsServer) return;
        _healMultiply.Value = Mathf.Max(0, newHealMultiply);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetDamageMultiplyServerRpc(float newShieldMultiply) {
        if (!IsServer) return;
        _shieldMultiply.Value = Mathf.Max(0, newShieldMultiply);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetShieldMultiplyServerRpc(float newDamageMultiply) {
        if (!IsServer) return;
        _damageMultiply.Value = Mathf.Clamp(newDamageMultiply, 0, 3);
    }

    #endregion
}
