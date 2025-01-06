using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageIncrease", menuName = "HealthDebuffs/DamageIncrease")]
public class DamageIncrease : HealthDebuff
{
    [Range(0, 1)][SerializeField] float increasedDamage;
    [SerializeField] float duration;
    public override IEnumerator ApplyDebuff(HealthManager health, int currentStacks) {
        health.SetMultiplyServerRpc(HealthMultipliers.Damage,1 + increasedDamage);
        yield return new WaitForSeconds(duration);
        StopDebuff(health);
    }
    public override void StopDebuff(HealthManager health) {
        health.SetMultiplyServerRpc(HealthMultipliers.Damage, 1 / (1 + increasedDamage));
        health.RemoveDebuff(this);
    }
}
