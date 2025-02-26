using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ReduceShield", menuName = "HealthDebuffs/ReduceShield")]
public class ReduceShield : HealthDebuff
{
    [Range(0, 1)][SerializeField] float reducedShieldPercent;
    [SerializeField] float duration;
    public override IEnumerator ApplyDebuff(HealthManager health, int currentStacks) {
        if (health.IsServer) health.SetMultiplyServerRpc(HealthMultipliers.Shield, 1 - reducedShieldPercent);
        yield return new WaitForSeconds(duration);
        StopDebuff(health);
    }
    public override void StopDebuff(HealthManager health) {
        health.SetMultiplyServerRpc(HealthMultipliers.Damage, 1 / (1 - reducedShieldPercent));
        health.RemoveDebuff(this);
    }
}
