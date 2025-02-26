using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ReduceHeal", menuName = "HealthDebuffs/ReduceHeal")]
public class ReduceHeal : HealthDebuff {
    [Range(0,1)][SerializeField] float reducedHealPercent;
    [SerializeField] float duration;
    public override IEnumerator ApplyDebuff(HealthManager health, int currentStacks) {
        if (health.IsServer) health.SetMultiplyServerRpc(HealthMultipliers.Heal, 1 - reducedHealPercent);
        yield return new WaitForSeconds(duration);
        StopDebuff(health);
    }
    public override void StopDebuff(HealthManager health) {
        health.SetMultiplyServerRpc(HealthMultipliers.Heal, 1 / (1 - reducedHealPercent));
        health.RemoveDebuff(this);
    }
}
