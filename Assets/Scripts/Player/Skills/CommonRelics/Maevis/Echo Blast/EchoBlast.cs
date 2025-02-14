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

    [Header("Echo Blast Level 2")]
    public float StunTime;
    public float ExplosionRadiusLevel2;

    [Header("Echo Blast Level 3")]
    public float ExplosionAmountLevel3;

    [Header("Echo Blast Level 4")]
    public HealthDebuff BleedDebuff;
    public float ExplodingDebuffDuration;
    public float ExplodingDebuffExplosionCooldown;
}
