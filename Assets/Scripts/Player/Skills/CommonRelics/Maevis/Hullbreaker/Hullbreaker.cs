using UnityEngine;

[CreateAssetMenu(menuName = "CommonRelic/Hullbreaker")]
public class Hullbreaker : CommonRelic
{
    [Header("Hullbreaker Level 1")]
    public float ShieldAmount;
    public float ShieldDuration;

    [Header("Hullbreaker Level 2")]
    public float ShieldAmountLevel2;

    [Header("Hullbreaker Level 3")]
    public float ExplosionDamage;
    public float ExplosionDuration;

    [Header("Hullbreaker Level 4")]
    public float ShieldDurationLevel4;
    public float ShieldAmountLevel4;
    public float EarthquakeDamage;
    public float EarthquakeDuration;
    public float EarthquakeInterval;
}
