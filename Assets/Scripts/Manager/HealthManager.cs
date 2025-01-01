using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class HealthManager : NetworkBehaviour {

    [Header("Atributes")]
    [SerializeField] private NetworkVariable<float> maxHealth;
    [SerializeField] private float maxShieldAmount;


    readonly NetworkVariable<float> _currentHealth = new();
    readonly NetworkVariable<float> _currentShieldAmount = new();
    readonly NetworkVariable<bool> _canTakeDamage = new(true);
    readonly NetworkVariable<bool> _isShielded = new(false);

    Coroutine _timeToEndShieldCoroutine;
    Coroutine _timeToEndInvulnerabilityCoroutine;

    void Start() {
        if (IsServer) {
            _currentHealth.Value = maxHealth.Value;
        }
    }
    public void TakeDamage(float damageTaken) {
        if (!IsServer) return;
        if (!_canTakeDamage.Value) { Debug.Log("Can't take Damage" + gameObject.name); return; }

        if (_isShielded.Value) {
            _currentShieldAmount.Value -= damageTaken;
            _currentHealth.Value = Mathf.Max(0, _currentHealth.Value);

            if (_currentShieldAmount.Value == 0) {
                float overDamage = -_currentShieldAmount.Value;
                _currentShieldAmount.Value = 0f;
                _isShielded.Value = false;
                _currentHealth.Value = Mathf.Clamp((_currentHealth.Value - overDamage), 0, maxHealth.Value);
            }
        }
        else {
            _currentHealth.Value = Mathf.Clamp((_currentHealth.Value - damageTaken), 0, maxHealth.Value);
        }
        if (_currentHealth.Value <= 0) {
            Death();
        }
    }
    void Death() {
        Debug.Log("Is dead: " + gameObject.name);
    }
    public void Revive(float percentOfMaxHealth) {
        _currentHealth.Value = Mathf.Clamp((percentOfMaxHealth / 100 * maxHealth.Value), 0.2f * maxHealth.Value, maxHealth.Value);
        _canTakeDamage.Value = true;
        _currentShieldAmount.Value = 0f;
        _isShielded.Value = false;
    }

    public void SetInvulnerability(bool state, float duration) {
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
        if (isCumulative) _currentShieldAmount.Value = Mathf.Clamp((_currentShieldAmount.Value + shieldAmount), 0, maxShieldAmount);
        else _currentShieldAmount.Value = Mathf.Clamp(shieldAmount, 0, maxShieldAmount);

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
        _currentHealth.Value = Mathf.Clamp((_currentHealth.Value + healAmount), 0, maxHealth.Value);
    }
    public void SetMaxHealth(float newMaxHealthValue) {
        if (IsServer) {
            float percentageOfCurrentHealth = _currentHealth.Value/maxHealth.Value;
            maxHealth.Value = newMaxHealthValue;
            _currentHealth.Value = Mathf.Min(percentageOfCurrentHealth * maxHealth.Value, maxHealth.Value);
        }
    }
}
