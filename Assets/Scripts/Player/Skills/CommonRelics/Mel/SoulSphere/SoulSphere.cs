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

}
