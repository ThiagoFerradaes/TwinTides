using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "HealOverTime", menuName = "HealthBuff/HealOverTime")]
public class HealOverTime : HealthBuff {
    [SerializeField] float healPerTick;
    [SerializeField] float amountOfTicks;
    [SerializeField] float timeBetweenTicks;
    public override IEnumerator ApplyBuff(HealthManager health, int currentStacks) {
        for(int i = 0; i < amountOfTicks; i++) {
            yield return new WaitForSeconds(timeBetweenTicks);
            if (health.IsServer) health.HealServerRpc(healPerTick);
        }
        StopBuff(health);
    }

    public override void StopBuff(HealthManager health) {
        health.RemoveBuff(this);
    }
}
