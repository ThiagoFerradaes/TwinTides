using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ReduceShield", menuName = "HealthDebuffs/ReduceShield")]
public class ReduceShield : HealthDebuff
{
    [Range(0, 1)][SerializeField] float reducedShieldPercent;
    [SerializeField] float duration;
    public override IEnumerator ApplyDebuff(HealthManager health, int currentStacks) {
        health.SetShieldMultiply(1 - reducedShieldPercent);
        yield return new WaitForSeconds(duration);
        health.SetShieldMultiply(1f);
        health.RemoveDebuff(this);
    }
}
