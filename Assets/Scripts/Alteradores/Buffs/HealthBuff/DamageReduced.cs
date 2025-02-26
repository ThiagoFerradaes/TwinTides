using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageReduced", menuName = "HealthBuff/DamageReduced")]
public class DamageReduced : HealthBuff {
    [Range(0, 1)][SerializeField] float reduceDamage;
    public override IEnumerator ApplyBuff(HealthManager health, int currentStacks) {
        if (health.IsServer) health.SetMultiplyServerRpc(HealthMultipliers.Damage, 1 - reduceDamage);
        yield return new WaitForSeconds(duration);
        StopBuff(health);
    }

    public override void StopBuff(HealthManager health) {
        health.SetMultiplyServerRpc(HealthMultipliers.Damage, 1 / (1 - reduceDamage));
        health.RemoveBuff(this);
    }
}
