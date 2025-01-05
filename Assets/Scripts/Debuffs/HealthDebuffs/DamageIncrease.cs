using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageIncrease", menuName = " HealthDebuff/DamageIncrease")]
public class DamageIncrease : HealthDebuff
{
    [Range(0, 1)][SerializeField] float increasedDamage;
    [SerializeField] float duration;
    public override IEnumerator ApplyDebuff(HealthManager health, int currentStacks) {
        health.SetDamageMultiplyServerRpc(1 + increasedDamage);
        yield return new WaitForSeconds(duration);
        health.SetHealMultiplyServerRpc(1f);
        health.RemoveDebuff(this);
    }
}
