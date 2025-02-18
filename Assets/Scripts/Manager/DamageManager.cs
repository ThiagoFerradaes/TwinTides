using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class DamageManager : NetworkBehaviour {

    [SerializeField] private NetworkVariable<float> baseAttack = new(0);
    [SerializeField] private NetworkVariable<float> baseNormalAttackCooldown = new(0);


    #region Métodos Relacionados ao Ataque Base
    public float ReturnBaseAttack() {
        return baseAttack.Value;
    }

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
    public float ReturnNormalAttackCooldown() {
        return baseNormalAttackCooldown.Value;
    }

    [Rpc(SendTo.Server)]
    public void IncreaseAttackCooldownRpc(float damageIncreaseMultiplier) {
        baseNormalAttackCooldown.Value *= damageIncreaseMultiplier;
    }

    [Rpc(SendTo.Server)]
    public void DecreaseAttackCooldownRpc(float damageDecreaseMultiplier) {
        baseNormalAttackCooldown.Value /= damageDecreaseMultiplier;
    }

    [Rpc(SendTo.Server)]
    public void IncreaseAttackCooldownRpc(float damageMultiplier, float duration) {
        StartCoroutine(IncreaseAttackCoroutine(damageMultiplier, duration));
    }

    [Rpc(SendTo.Server)]
    public void DecreaseAttackCooldownWithTimeRpc(float damageMultiplier, float duration) {
        StartCoroutine(DecreaseAttackCoroutine(damageMultiplier, duration));
    }

    IEnumerator IncreaseAttackCooldownCoroutine(float damageMultipiler, float duration) {
        baseNormalAttackCooldown.Value *= damageMultipiler;
        yield return new WaitForSeconds(duration);
        baseNormalAttackCooldown.Value /= damageMultipiler;
    }
    IEnumerator DecreaseAttackCooldownCoroutine(float damageMultipiler, float duration) {
        baseNormalAttackCooldown.Value /= damageMultipiler;
        yield return new WaitForSeconds(duration);
        baseNormalAttackCooldown.Value *= damageMultipiler;
    }

    #endregion
}
