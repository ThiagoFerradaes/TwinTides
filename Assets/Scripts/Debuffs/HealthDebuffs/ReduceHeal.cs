using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ReduceHeal", menuName = "HealthDebuffs/ReduceHeal")]
public class ReduceHeal : HealthDebuff {
    [Range(0,1)][SerializeField] float reducedHealPercent;
    [SerializeField] float duration;
    public override IEnumerator ApplyDebuff(HealthManager health, int currentStacks) {
        health.SetHealMultiply(1 - reducedHealPercent);
        yield return new WaitForSeconds(duration);
        health.SetHealMultiply(1f);
        health.RemoveDebuff(this);
    }
}
