using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "HealBlocker", menuName = "HealthDebuffs/HealBlocker")]
public class HealBlocker : HealthDebuff
{
    [SerializeField] float duration;
    public override IEnumerator ApplyDebuff(HealthManager health, int currentStacks) {
        health.SetPermissionServerRpc(HealthPermissions.CanBeHealed, false);
        yield return new WaitForSeconds(duration);
        StopDebuff(health);
    }
    public override void StopDebuff(HealthManager health) {
        health.SetPermissionServerRpc(HealthPermissions.CanBeHealed, true);
        health.RemoveDebuff(this);
    }
}
