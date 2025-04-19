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
                if (health.IsServer) health.DealDamage(damagePerTickOnShield * Mathf.Max(minimumShieldAmountToMultiplyDamage,
                    health.currentShieldAmount.Value), true, false);
                yield return new WaitForSeconds(timeBetweenDamage);
            }
            else {
                if (health.IsServer) health.DealDamage(damagePerTickOffShield * health.ReturnMaxHealth(), true, false);
                yield return new WaitForSeconds(timeBetweenDamage);
            } 
        }

        StopDebuff(health);
    }
    public override void StopDebuff(HealthManager health) {
        health.RemoveDebuff(this);
    }
}
