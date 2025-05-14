using UnityEngine;

[CreateAssetMenu (menuName = "BlackBeardAttack/ Cannon Attack")]
public class BlackBeardCannon : BlackBeardSO
{
    [Header("Phase Atributes")]
    public float InitialTime;
    public float CooldownBetweenAttacks;
    public float MaxAmountOfAttacksToBomb;
    [Range(0,100)]public float PercentToChangeState;

    [Header("Shoot Up")]
    public float AmountOfShootUpBullets;
    public float ShootUpBulletHeigh;
    public float ShootUpBulletSpeedToFall;
    public float ShootUpBulletDamage;
    public float TimeBetweenShootUpBullets;
    public float ShootUpRadius;
    public float ShootUpBulletSize;
    public float DistanceBetweenEachShootUpBullet;
    public float ShootUpBulletWarningDuration;
    public float ShootUpBulletWarningSize;

    [Header("Shoot Straight")]
    public float AmountOfStraighBullets;
    public float StraighBulletSpeed;
    public float StraighBulletDamage;
    public float TimeBetweenStraighBullets;
    public float StraighBulletRange;

    [Header("Shoot Bomb")]
    public float AmountOfBombs;
    public float BombHeight;
    public float BombFallSpeed;
    public float BombExplosionRadius;
    public float BombExplosionDamage;
    public float TimeBetweenBombs;
    public float TimeToBombExplode;
    public float BombDamageToShip;
}
