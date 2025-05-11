using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAttack/ Girl Two Three")]
public class GirlTwoThree : EnemyAttack
{
    [Header("Atributes")]
    public float timeBetweenDamage;
    public float offSett;
    public float cooldown;

    [Header("High Damage Puddle")]
    public float highDamagePuddleDamageToHealth;
    public float highDamagePuddleDamageToShield;
    public float highDamagePuddleDuration;
    public float highDamagePuddleSize;
    public Material highDamagePuddleMaterial;

    [Header("Slow and Stun Puddle")]
    public float slowPuddleDamage;
    public float slowPuddleSlowPercent;
    public float slowPuddleDuration;
    public float slowPuddleSize;
    public float slowPuddleStunDuration;
    public Material slowPuddleMaterial;

    [Header("No Buffs Puddle")]
    public float blockPuddleDamage;
    public float blockPuddleDuration;
    public float blockPuddleSize;
    public HealthDebuff blockPuddleNoDebuffDebuff;
    public Material blockPuddleMaterial;
}
