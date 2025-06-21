using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class DamageManager : NetworkBehaviour {

    [SerializeField] float baseAttackSpeed = 1;
    [SerializeField] float baseAttack;
    private NetworkVariable<float> attack = new(0);
    public NetworkVariable<float> attackSpeed = new();

    #region Métodos Relacionados ao Ataque Base
    public float ReturnBaseAttack() {
        return attack.Value;
    }

    public float ReturnTotalAttack(float skillDamage) {
        return (1 + attack.Value) * skillDamage;
    }


    /// <summary>
    /// Multiplica o valor do ataque base. ELe normalmente é 1, e pode ir até 2
    /// </summary>
    /// <param name="damageIncreaseMultiplier"></param>
    public void IncreaseBaseAttack(float damageIncreaseMultiplier) {
        if (IsServer)
            attack.Value *= damageIncreaseMultiplier;
    }

    public void DecreaseBaseAttack(float damageDecreaseMultiplier) {
        if (IsServer)
            attack.Value /= damageDecreaseMultiplier;
    }

    public void IncreaseBaseAttackWithTime(float damageMultiplier, float duration) {
        if (IsServer)
            StartCoroutine(IncreaseAttackCoroutine(damageMultiplier, duration));
    }

    public void DecreaseBaseAttackWithTime(float damageMultiplier, float duration) {
        if (IsServer)
            StartCoroutine(DecreaseAttackCoroutine(damageMultiplier, duration));
    }

    IEnumerator IncreaseAttackCoroutine(float damageMultipiler, float duration) {
        attack.Value *= damageMultipiler;
        yield return new WaitForSeconds(duration);
        attack.Value /= damageMultipiler;
    }
    IEnumerator DecreaseAttackCoroutine(float damageMultipiler, float duration) {
        attack.Value /= damageMultipiler;
        yield return new WaitForSeconds(duration);
        attack.Value *= damageMultipiler;
    }

    #endregion

    #region Métodos Relacionados ao Cooldown do Ataque/ Velocidade do Ataque

    public float ReturnAttackSpeed() => attackSpeed.Value;
    public float ReturnBaseAttackSpeed() => baseAttackSpeed;
    /// <summary>
    /// Retorna a divisão do número passado pela velocidade de ataque atual
    /// </summary>
    /// <returns></returns>
    public float ReturnDivisionAttackSpeed(float numberToBeDivided) {
        return numberToBeDivided / Mathf.Max(attackSpeed.Value, 0.1f);
    }

    /// <summary>
    /// Retorna a multiplicação do número passado pela velocidade de ataque atual
    /// </summary>
    /// <returns></returns>
    public float ReturnMultipliedAttackSpeed(float numberToBeMultiplied) {
        return numberToBeMultiplied * attackSpeed.Value;
    }


    /// <summary>
    /// Aumenta a velocidade de ataque em X% 
    /// </summary>
    /// <param name="percentToIncrease"></param>
    public void IncreaseAttackSpeed(float percentToIncrease) {
        if (IsServer)
            attackSpeed.Value *= (1 + percentToIncrease / 100);
    }

    /// <summary>
    /// Diminui o cooldown base dos ataques normais. A função divide o cooldown pelo DamageDecreaseMultiplier
    /// </summary>
    /// <param name="percentToDecrease"></param>
    public void DecreaseAttackSpeed(float percentToDecrease) {
        if (IsServer)
            attackSpeed.Value /= (1 + percentToDecrease / 100);
    }

    public void IncreaseAttackSpeedWithTime(float attackSpeedPercent, float duration) {
        if (IsServer)
            StartCoroutine(IncreaseAttackSpeedCoroutine((1 + attackSpeedPercent / 100), duration));
    }

    public void DecreaseAttackSpeedWithTime(float attackSpeedPercent, float duration) {
        if (IsServer)
            StartCoroutine(DecreaseAttackSpeedCoroutine((1 + attackSpeedPercent / 100), duration));
    }

    IEnumerator IncreaseAttackSpeedCoroutine(float attackSpeedMultiplier, float duration) {
        attackSpeed.Value *= attackSpeedMultiplier;
        yield return new WaitForSeconds(duration);
        attackSpeed.Value /= attackSpeedMultiplier;
    }
    IEnumerator DecreaseAttackSpeedCoroutine(float damageMultipiler, float duration) {
        attackSpeed.Value /= damageMultipiler;
        yield return new WaitForSeconds(duration);
        attackSpeed.Value *= damageMultipiler;
    }

    #endregion

    #region Reset

    private void Start() {
        if (IsServer) {
            PlayersDeathManager.OnGameRestart += ResetStats;
            attackSpeed.Value = baseAttackSpeed;
            attack.Value = baseAttack;
        }
    }
    public void ResetStats() {
        if (!IsServer) return;

        StopAllCoroutines();
        attack.Value = baseAttack;
        attackSpeed.Value = baseAttackSpeed;
       
    }

    public override void OnDestroy() {
        if (IsServer) PlayersDeathManager.OnGameRestart -= ResetStats;
    }

    #endregion
}
