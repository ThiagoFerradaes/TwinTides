using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAttack/ Zombie Two Two")]
public class ZombieTwoTwo : EnemyAttack
{
    [Header("Attack")]
    public float attackDamage;
    public float stunTime;
    public float attackCooldown;
    public float hitBoxSize;

    [Header("Dash")]
    public float dashSpeed;
    public float dashDistance;
}
