using UnityEngine;

[CreateAssetMenu(menuName = "BlackBeardAttack/ Bullet Rain")]
public class BlackBeardBulletRainSO : BlackBeardSO
{
    [Header("Attack Over 50%")]
    public int AmountOfBullets;
    public float BulletSize;
    public float BulletHeight;
    public float BulletFallSpeed;
    public float Radius;
    public HealthDebuff BurningDebuff;
    public float TimeBetweenEachBullet;
    public float ExplosionDamage;
    public float FieldDamage;
    public Vector2 FieldSize;
    public float TimeBetweenFieldDamage;
    public float FieldDuration;
    public float Cooldown;
    public float AttackTime;
    public float ExplosionRadius;
    public float ExplosionDuration;

    [Header("Attack Under 50%")]
    public int AmountOfBulletsStronger;
    public int AmountOfSecondaryBullets;
    public float RadiusStronger;
    public float SecondaryBulletDistanceToMainBullet;
    public float SecondaryBulletSize;
    public float SecondaryExplosionRadiusPercent;
    public float SecondaryBulletJumpPower;
    public float SecondaryBulletJumpDuration;
    public float SecondaryBulletSizeMultiplier;
}
