using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ShieldBlocker", menuName = "HealthDebuffs/ShieldBlocker")]
public class ShieldBlocker : HealthDebuff {
    [SerializeField] float duration;
    public override IEnumerator ApplyDebuff(HealthManager health, int currentStacks) {
        health.SetPermissionServerRpc(HealthPermissions.CanBeShielded, false);
        yield return new WaitForSeconds(duration);
        StopDebuff(health);
    }
    public override void StopDebuff(HealthManager health) {
        health.SetPermissionServerRpc(HealthPermissions.CanBeShielded, true);
        health.RemoveDebuff(this);
    }
}
