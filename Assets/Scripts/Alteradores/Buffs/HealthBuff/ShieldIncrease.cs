using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "HealIncrease", menuName = "HealthBuff/ShieldIncrease")]
public class ShieldIncrease : HealthBuff {
    [Range(0, 1)][SerializeField] float increaseShield;
    public override IEnumerator ApplyBuff(HealthManager health, int currentStacks) {
        health.SetMultiplyServerRpc(HealthMultipliers.Shield, 1 + increaseShield);
        yield return new WaitForSeconds(duration);
        StopBuff(health);
    }

    public override void StopBuff(HealthManager health) {
        health.SetMultiplyServerRpc(HealthMultipliers.Shield, 1 / (1 + increaseShield));
        health.RemoveBuff(this);
    }
}
