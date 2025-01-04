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
                health.ApplyDamageOnServerRPC(damagePerTickOnShield * currentStacks, true);
                yield return new WaitForSeconds(timeBetweenDamage);
            }
            else {
                health.ApplyDamageOnServerRPC(damagePerTickOffShield * currentStacks, true);
                yield return new WaitForSeconds(timeBetweenDamage);
            }
        }

        health.RemoveDebuff(this);
    }
}
