using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Poison", menuName = "HealthDebuffs/Poison")]
public class Poison : HealthDebuff {

    [Range(0, 1)][SerializeField] float damagePerTick;
    [SerializeField] float timeBetweenDamage;
    [SerializeField] int amountOfTicks;
    public override IEnumerator ApplyDebuff(HealthManager health, int currentStacks) {
        for (int i = 0; i < amountOfTicks; i++) {
            yield return new WaitForSeconds(timeBetweenDamage);
            health.ApplyDamageOnServerRPC((damagePerTick * currentStacks * health.ReturnMaxHealth()), false, false);
        }
        StopDebuff(health);
    }
    public override void StopDebuff(HealthManager health) {
        health.RemoveDebuff(this);
    }
}
