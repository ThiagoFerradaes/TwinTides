using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Burn", menuName = "HealthDebuffs/Burn")]
public class Burn : HealthDebuff
{
    [SerializeField] float minimumShieldAmountToMultiplyDamage;
    [Range(0,1)][SerializeField] float damagePerTickOnShield;
    [Range(0, 1)][SerializeField] float damagePerTickOffShield;
    [SerializeField] float timeBetweenDamage;
    [SerializeField] int amountOfTicks;
    public override IEnumerator ApplyDebuff(HealthManager health, int currentStacks) {
        for (int i = 0; i < amountOfTicks; i++) {
            if (health.isShielded.Value) {
                health.ApplyDamageOnServerRPC(damagePerTickOnShield * Mathf.Max(minimumShieldAmountToMultiplyDamage,
                    health.currentShieldAmount.Value), true);
                yield return new WaitForSeconds(timeBetweenDamage);
            }
            else {
                health.ApplyDamageOnServerRPC(damagePerTickOffShield * health.maxHealth.Value, true);
                yield return new WaitForSeconds(timeBetweenDamage);
            } 
        }

        health.RemoveDebuff(this);
    }
}
