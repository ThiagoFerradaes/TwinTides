using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ReduceShield", menuName = "HealthDebuffs/ReduceShield")]
public class ReduceShield : HealthDebuff
{
    [Range(0, 1)][SerializeField] float reducedShieldPercent;
    [SerializeField] float duration;
    public override IEnumerator ApplyDebuff(HealthManager health, int currentStacks) {
        health.SetShieldMultiplyServerRpc(1 - reducedShieldPercent);
        yield return new WaitForSeconds(duration);
        health.SetShieldMultiplyServerRpc(1f);
        health.RemoveDebuff(this);
    }
}
