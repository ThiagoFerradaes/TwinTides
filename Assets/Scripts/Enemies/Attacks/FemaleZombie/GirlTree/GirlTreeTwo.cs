using FMODUnity;
using UnityEngine;

[CreateAssetMenu (menuName = "EnemyAttack/ GirlTreeTwo")]
public class GirlTreeTwo : EnemyAttack
{

    [Header("Bombs")]
    public float bombsSpeed;
    public float bombsRange;
    public float bombPlacement;
    public float amountOfBombs;
    public float timeBetweenBombs;
    public float cooldown;
    public EventReference ShootSound;

    [Header("Explosion")]
    public float explosionRadius;
    public float explosionDamage;
    public float explosionDuration;
    public HealthDebuff burningDebuff;
    public EventReference ExplosionSound;
}
