using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Bleed", menuName = "HealthDebuffs/Bleed")]
public class Bleed : HealthDebuff
{
    [SerializeField] float damagePerTickOnShield;
    [SerializeField] float damagePerTickOffShield;
    [SerializeField] float timeBetweenDamage;
    [SerializeField] int amountOfTicks;
    public override IEnumerator ApplyDebuff(HealthManager health, int currentStacks) {
        for (int i = 0; i < amountOfTicks; i++) {
            if (health.isShielded.Value) {
                if (health.IsServer) health.DealDamage(damagePerTickOnShield * currentStacks, true, true);
                yield return new WaitForSeconds(timeBetweenDamage);
            }
            else {
                health.DealDamage(damagePerTickOffShield * currentStacks, true, true);
                yield return new WaitForSeconds(timeBetweenDamage);
            }
        }

        StopDebuff(health);
    }
    public override void StopDebuff(HealthManager health) {
        health.RemoveDebuff(this);
    }
}
