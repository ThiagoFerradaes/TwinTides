using UnityEngine;

[CreateAssetMenu(menuName = "CommonRelic/SoulSphere")]
public class SoulSphere : CommonRelic {

    [Header("Sphere")]
    public float SphereDuration;
    public float SphereSpeed;
    public float DamagePassingThroughEnemy;
    public HealthBuff invulnerabilityBuff;

    [Header("Explosion")]
    public float ExplosionDuration;
    public float ExplosionRadius;
    public float ExplosionRadiusMultiplier;
    public float ExplosionDamage;

    [Header("Area")]
    public float AreaDurationLevel3;
    public float AreaDurationLevel4;
    public Vector3 AreaRadius;
    public Vector3 AreaRadiusLevel4;

}
