using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "HealIncrease", menuName = "HealthBuff/HealIncrease")]
public class HealIncrease : HealthBuff
{
    [Range(0, 1)][SerializeField] float increaseHeal;
    public override IEnumerator ApplyBuff(HealthManager health, int currentStacks) {
        health.SetMultiplyServerRpc(HealthMultipliers.Heal, 1 + increaseHeal);
        yield return new WaitForSeconds(duration);
        StopBuff(health);
    }

    public override void StopBuff(HealthManager health) {
        health.SetMultiplyServerRpc(HealthMultipliers.Heal, 1 / (1 + increaseHeal));
        health.RemoveBuff(this);
    }
}
