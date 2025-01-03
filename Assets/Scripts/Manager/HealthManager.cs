using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthManager : NetworkBehaviour {

    [Header("Atributes")]
    [SerializeField] private NetworkVariable<float> maxHealth;
    [SerializeField] private float maxShieldAmount;

    // Variaveis do tipo float
    readonly NetworkVariable<float> _currentHealth = new();
    readonly NetworkVariable<float> _currentShieldAmount = new();
    readonly NetworkVariable<float> _healMultiply = new(1f);
    readonly NetworkVariable<float> _shieldMultiply = new(1f);  

    // Variaveis do tipo bool
    readonly NetworkVariable<bool> _canTakeDamage = new(true);
    readonly NetworkVariable<bool> _isShielded = new(false);
    readonly NetworkVariable<bool> _canReceiveShield = new(true);
    readonly NetworkVariable<bool> _canBeHealed = new(true);
    readonly NetworkVariable<bool> _canBeInvulnerable = new(true);

    // Lista de debuffs atuais
    readonly Dictionary<Type, (int , IEnumerator)> _listOfActiveDebuffs = new();

    Coroutine _timeToEndShieldCoroutine;
    Coroutine _timeToEndInvulnerabilityCoroutine;

    // Eventos
    public event Action<(float maxHealth, float currentHealth)> UpdateHealth;
    public event Action OnDeath;

    void Start() {
        if (IsServer) {
            _currentHealth.Value = maxHealth.Value;
        }
        UpdateHealth((maxHealth.Value, _currentHealth.Value));
    }

    public void AddDebuffToList(HealthDebuff debuff) {
        if (_listOfActiveDebuffs.ContainsKey(debuff.GetType())) {
            StopCoroutine(_listOfActiveDebuffs[debuff.GetType()].Item2); // paramos a corrotina do debuff
            var stacks = _listOfActiveDebuffs[debuff.GetType()].Item1;
            stacks += debuff.AddStacks; // aumentamos o stack do debuff

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
    public void TakeDamage(float damageTaken, bool hitShield) {
        if (!IsServer) return;
        if (!_canTakeDamage.Value) { Debug.Log("Can't take Damage" + gameObject.name); return; }

        if (_isShielded.Value && hitShield) {
            _currentShieldAmount.Value -= damageTaken;
            _currentHealth.Value = Mathf.Max(0, _currentHealth.Value);

            if (_currentShieldAmount.Value == 0) {
                float overDamage = -_currentShieldAmount.Value;
                _currentShieldAmount.Value = 0f;
                _isShielded.Value = false;
                _currentHealth.Value = Mathf.Clamp((_currentHealth.Value - overDamage), 0, maxHealth.Value);
                UpdateHealth?.Invoke((maxHealth.Value, _currentHealth.Value));
            }
        }
        else {
            _currentHealth.Value = Mathf.Clamp((_currentHealth.Value - damageTaken), 0, maxHealth.Value);
            UpdateHealth?.Invoke((maxHealth.Value, _currentHealth.Value));
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
        _currentShieldAmount.Value = 0f;
        _isShielded.Value = false;
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

    public void ReceiveShield(float shieldAmount, float durationOfShield, bool isCumulative) {

        if (!_canReceiveShield.Value) return; // não pode receber escudo
        
        if (isCumulative) _currentShieldAmount.Value = Mathf.Clamp((_currentShieldAmount.Value + shieldAmount * _shieldMultiply.Value)
            , 0, maxShieldAmount); // o escudo recebido acumula com o escudo atual

        // o escudo recebido não acumula com o escudo atual
        else _currentShieldAmount.Value = Mathf.Clamp(shieldAmount * _shieldMultiply.Value, 0, maxShieldAmount);

        _isShielded.Value = _currentShieldAmount.Value > 0;

        if (_timeToEndShieldCoroutine != null) StopCoroutine(_timeToEndShieldCoroutine);
        _timeToEndShieldCoroutine = StartCoroutine(TimeToEndShield(durationOfShield));
    }
    IEnumerator TimeToEndShield(float time) {
        yield return new WaitForSeconds(time);
        _currentShieldAmount.Value = 0;
        _isShielded.Value = false;
    }
    public void Heal(float healAmount) {
        
        if (!_canBeHealed.Value) return;

        _currentHealth.Value = Mathf.Clamp((_currentHealth.Value + healAmount * _healMultiply.Value), 0, maxHealth.Value);
    }
    public void SetMaxHealth(float newMaxHealthValue) {
        if (IsServer) {
            float percentageOfCurrentHealth = _currentHealth.Value/maxHealth.Value;
            maxHealth.Value = newMaxHealthValue;
            _currentHealth.Value = Mathf.Min(percentageOfCurrentHealth * maxHealth.Value, maxHealth.Value);
        }
        UpdateHealth((maxHealth.Value, _currentHealth.Value));
    }
}
