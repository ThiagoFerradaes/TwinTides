using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "CommonRelic/EchoBlast")]
public class EchoBlast : CommonRelic
{
    [Header("Echo Blast Level 1")]
    public float BulletDuration;
    public float BulletSpeed;
    public float ExplosionRadius;
    public float ExplosionDuration;
    public float ExplosionDamage;
    public float ManagerDuration;
    public EventReference ShotSound;

    [Header("Echo Blast Level 2")]
    public float StunTime;
    public float ExplosionRadiusLevel2;
    public EventReference ExplosionSound;

    [Header("Echo Blast Level 3")]
    public float ExplosionAmountLevel3;
    public float TimeBetweenEachExplosion;

    [Header("Echo Blast Level 4")]
    public float ExplodingDebuffDuration;
    public float ExplodingDebuffExplosionCooldown;
    public float ExplodingDebuffDelay;
    public float ExplodingDebuffHeight;
    public EventReference SecondExplosionSound;
}
