using UnityEngine;

[CreateAssetMenu(menuName = "BlackBeardAttack/ Bullet Rain")]
public class BlackBeardBulletRainSO : BlackBeardSO
{
    [Header("Bullet")]
    public int AmountOfBullets;
    public int AmountOfBulletsStronger;
    public float BulletSize;
    public float BulletHeight;
    public float BulletFallSpeed;
    public float TimeBetweenEachBullet;
    public float AttackTime;
    public float AttackRadius;
    public float AttackRadiusStronger;

    [Header("Explosion")]
    public float ExplosionDamage;
    public float ExplosionRadius;
    public float ExplosionDuration;

    [Header("Field")]
    public float FieldDamage;
    public float TimeBetweenFieldDamage;
    public float FieldDuration;
    public HealthDebuff BurningDebuff;
    public Vector2 FieldSize;

    [Header("Secondary Bullet")]
    public int AmountOfSecondaryBullets;
    public float SecondaryBulletDistanceToMainBullet;
    public float SecondaryBulletSize;
    public float SecondaryExplosionRadiusPercent;
    public float SecondaryBulletJumpPower;
    public float SecondaryBulletJumpDuration;
    public float SecondaryBulletSizeMultiplier;
}
