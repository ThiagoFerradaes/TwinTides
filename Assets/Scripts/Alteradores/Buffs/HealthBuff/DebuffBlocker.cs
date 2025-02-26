using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DebuffBlocker", menuName = "HealthBuff/DebuffBlocker")]
public class DebuffBlocker : HealthBuff {
    public override IEnumerator ApplyBuff(HealthManager health, int currentStacks) {
        if (health.IsServer) health.SetPermissionServerRpc(HealthPermissions.CanBeDebuffed, false);
        yield return new WaitForSeconds(duration);
        StopBuff(health);
    }

    public override void StopBuff(HealthManager health) {
        health.SetPermissionServerRpc(HealthPermissions.CanBeDebuffed, true);
        health.RemoveBuff(this);
    }
}
