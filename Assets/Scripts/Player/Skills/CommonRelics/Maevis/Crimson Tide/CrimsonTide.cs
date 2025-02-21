using UnityEngine;

[CreateAssetMenu(menuName = "CommonRelic/CrimsonTide")]
public class CrimsonTide : CommonRelic
{
    [Header("Crimson Tide Level 1")]
    public float PunchDamage;
    public float PunchAreaDuration;
    public float PunchAreaOffSett;

    [Header("Crimson Tide Level 2")]
    public float DashDuration;
    public float DashSpeed;
    public float DashDamage;

    [Header("Crimson Tide Level 3")]
    public float ExplosionDamage;
    public float ExplosionDuration;
    public float ExplosionRadius;

    [Header("Crimson Tide Level 4")]
    public float PathDamagePerTick;
    public float PathDuration;
    public float PathDamageInterval;
    public float PathSpawnInterval;
    public Vector3 PathSize;
}
