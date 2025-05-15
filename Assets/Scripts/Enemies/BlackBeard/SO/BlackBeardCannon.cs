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
    public float AmountOfStraightBulletsAttacks;
    public float StraightBulletSpeed;
    public float StraightBulletDamage;
    [Tooltip("Esse aqui é o tempo entre as balas de tiros consecutivos de um ataque de uma sequencia")]
    public float TimeBetweenStraightBullets;
    [Tooltip("Esse aqui é o tempo entre ataques de sequencias diferentes, ou seja, ele faz um sequencia, espera esse tempo e depois faz outra")]
    public float TimeBetweenStraightBulletsAttacks;
    [Tooltip("Esse aqui é o tempo entre ataques da mesma sequencia, por exemplo se for 1, 2, 3, 4 e depois 4, 3, 2, 1 esse é o tempo entre essas duas sequências")]
    public float TimeBetweenStraightBulletsSequence;
    public float StraightBulletRange;
    public float StraightBulletStunTime;
    public float StraightBulletSize;
    public float CannonMovementSpeed;
    public float CannonMovementRange;

    [Header("Shoot Bomb")]
    public float AmountOfBombs;
    public float BombHeight;
    public float BombFallSpeed;
    public float BombExplosionRadius;
    public float BombExplosionDamage;
    public float BombExplosionDuration;
    public float TimeBetweenBombs;
    public float TimeToBombExplode;
    public float BombDamageToShip;
    public float ShootBombRadius;
    public float BombSize;
    public float BombSpeed;
    public float DistanceBetweenBombs;
    public float CooldownAfterBomb;
    public float TimeBetweenWarnings;
    public float WarningCooldown;
    public Material WarningMaterial;
}
