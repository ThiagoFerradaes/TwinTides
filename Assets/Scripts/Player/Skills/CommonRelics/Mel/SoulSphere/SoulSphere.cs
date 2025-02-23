using UnityEngine;

[CreateAssetMenu(menuName = "CommonRelic/SoulSphere")]
public class SoulSphere : CommonRelic {

    [Header("SoulSphere Level 1")]
    public float SphereDuration;
    public float SphereSpeed;
    public float DamagePassingThroughEnemy;
    public HealthBuff invulnerabilityBuff;

    [Header("SoulSphere Level 2")]
    public float ExplosionDuration;
    public float ExplosionRadius;
    public float ExplosionDamage;

    [Header("SoulSphere Level 3")]
    public float AreaDurationLevel3;
    public Vector3 AreaRadius;
    public float AreaDamage;
    public float AreaDamageCooldown;

    [Header("SoulSphere Level 4")]
    public float ExplosionRadiusLevel4;
    public float AreaDurationLevel4;
    public Vector3 AreaRadiusLevel4;
}
