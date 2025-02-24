using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class DamageManager : NetworkBehaviour {

    [SerializeField] private NetworkVariable<float> baseAttack = new(0);
    [SerializeField] private NetworkVariable<float> attackSpeed = new(1);

    #region Métodos Relacionados ao Ataque Base
    public float ReturnBaseAttack() {
        return baseAttack.Value;
    }

    public float ReturnTotalAttack(float skillDamage) {
        return (1 + baseAttack.Value) * skillDamage;
    }

    /// <summary>
    /// Multiplica o valor do ataque base. ELe normalmente é 1, e pode ir até 2
    /// </summary>
    /// <param name="damageIncreaseMultiplier"></param>
    [Rpc(SendTo.Server)]
    public void IncreaseBaseAttackRpc(float damageIncreaseMultiplier) {
        baseAttack.Value *= damageIncreaseMultiplier;
    }

    [Rpc(SendTo.Server)]
    public void DecreaseBaseAttackRpc(float damageDecreaseMultiplier) {
        baseAttack.Value /= damageDecreaseMultiplier;
    }

    [Rpc(SendTo.Server)]
    public void IncreaseBaseAttackWithTimeRpc(float damageMultiplier, float duration) {
        StartCoroutine(IncreaseAttackCoroutine(damageMultiplier, duration));
    }

    [Rpc(SendTo.Server)]
    public void DecreaseBaseAttackWithTimeRpc(float damageMultiplier, float duration) {
        StartCoroutine(DecreaseAttackCoroutine(damageMultiplier, duration));
    }

    IEnumerator IncreaseAttackCoroutine(float damageMultipiler, float duration) {
        baseAttack.Value *= damageMultipiler;
        yield return new WaitForSeconds(duration);
        baseAttack.Value /= damageMultipiler;
    }
    IEnumerator DecreaseAttackCoroutine(float damageMultipiler, float duration) {
        baseAttack.Value /= damageMultipiler;
        yield return new WaitForSeconds(duration);
        baseAttack.Value *= damageMultipiler;
    }

    #endregion

    #region Métodos Relacionados ao Cooldown do Ataque/ Velocidade do Ataque
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
    [Rpc(SendTo.Server)]
    public void IncreaseAttackSpeedRpc(float percentToIncrease) {
        attackSpeed.Value *=(1 + percentToIncrease/100);
    }

    /// <summary>
    /// Diminui o cooldown base dos ataques normais. A função divide o cooldown pelo DamageDecreaseMultiplier
    /// </summary>
    /// <param name="percentToDecrease"></param>
    [Rpc(SendTo.Server)]
    public void DecreaseAttackSpeedRpc(float percentToDecrease) {
        attackSpeed.Value /= (1 + percentToDecrease/100);
    }

    [Rpc(SendTo.Server)]
    public void IncreaseAttackSpeedWithTimeRpc(float attackSpeedPercent, float duration) {
        StartCoroutine(IncreaseAttackSpeedCoroutine((1 + attackSpeedPercent/100), duration));
    }

    [Rpc(SendTo.Server)]
    public void DecreaseAttackSpeedWithTimeRpc(float attackSpeedPercent, float duration) {
        StartCoroutine(DecreaseAttackSpeedCoroutine((1 + attackSpeedPercent/100), duration));
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
}
