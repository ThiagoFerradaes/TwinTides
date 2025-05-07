using UnityEngine;

public class GirlXOne : EnemyAttack
{
    [Header("Bullet")]
    public float amountOfBullets;
    public float timeBetweenBullets;
    public float amountOfSequencesToShotStrongerBullet;
    public float damageOfBullet;
    public float cooldownOfBullets;
    public float bulletSpeed;
    public float bulletRange;
    public float placementOfBullet;

    [Header("Stronger Bullet")]
    public float StrongerBulletDamage;
    public float StrongerBulletSpeed;
    public float StrongerBulletRange;
    public float StrongerBulletCooldown;
}
