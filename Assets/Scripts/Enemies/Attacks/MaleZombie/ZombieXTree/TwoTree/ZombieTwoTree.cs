using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAttack/ Zombie Two Tree")]
public class ZombieTwoTree : EnemyAttack
{
    [Header("Rain")]
    public float amountOfbombs;
    public float rainRadius;
    public float timeBetweenEachBomb;
    public float rainCooldown;
    public float durationOfAttack;

    [Header("Bomb")]
    public float fallSpeed;
    public float bombHeight;

    [Header("Explosion")]
    public float explosionSize;
    public float explosionDamage;
    public float explosionDuration;
    public HealthDebuff poisonDebuff;
}
