using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Poison", menuName = "HealthDebuffs/Poison")]
public class Poison : HealthDebuff {

    [Range(0, 1)][SerializeField] float damagePerTick;
    [SerializeField] float timeBetweenDamage;
    [SerializeField] int amountOfTicks;
    public override IEnumerator ApplyDebuff(HealthManager health, int currentStacks) {
        for (int i = 0; i < amountOfTicks; i++) {
            health.ApplyDamageOnServerRPC((damagePerTick * currentStacks * health.maxHealth.Value), false);
            yield return new WaitForSeconds(timeBetweenDamage);
        }

        health.RemoveDebuff(this);
    }
}
