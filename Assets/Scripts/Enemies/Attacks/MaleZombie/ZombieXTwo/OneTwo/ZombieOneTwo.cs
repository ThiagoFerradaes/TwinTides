using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAttack/ Zombie One Two")]
public class ZombieOneTwo : EnemyAttack
{
    [Header("Jump Attack")]
    public float distanceToJump;
    public float jumpHeight;
    public float jumpDuration;
    public float jumpDamage;
    public float jumpExplosionDuration;
    public float jumpExplosionRadius;
    public float cooldown;

    [Header("Normal Attack")]
    public float normalAttackSize;
    public float normalAttackDuration;
    public float normalAttackDamage;
}
