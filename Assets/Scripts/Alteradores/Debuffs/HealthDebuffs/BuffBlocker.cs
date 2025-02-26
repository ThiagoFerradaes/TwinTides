using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffBlocker", menuName = "HealthDebuffs/BuffBlocker")]
public class BuffBlocker : HealthDebuff {
    [SerializeField] float duration;
    public override IEnumerator ApplyDebuff(HealthManager health, int currentStacks) {
        if (health.IsServer) health.SetPermissionServerRpc(HealthPermissions.CanBeBuffed, false);
        yield return new WaitForSeconds(duration);
        StopDebuff(health);
    }
    public override void StopDebuff(HealthManager health) {
        health.SetPermissionServerRpc(HealthPermissions.CanBeBuffed, true);
        health.RemoveDebuff(this);
    }
}
