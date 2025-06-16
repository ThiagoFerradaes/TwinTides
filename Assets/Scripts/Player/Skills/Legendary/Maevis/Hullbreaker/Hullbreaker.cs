using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "LegendaryRelic/Hullbreaker")]
public class Hullbreaker : LegendaryRelic
{
    [Header("Atributes")]
    public float ShieldDuration;
    public float ShieldAmount;
    public float ExplosionDamage;
    public float ExplosionDuration;
    public float EarthquakeDamage;
    public float ExplosionRadius;
    public float EarthquakeDuration;
    public float EarthquakeInterval;
    public float EarthquakeRadius;
    public EventReference EarthquakeSound;
    public EventReference ExplosionSound;

    [Header("Animation")]
    public string animationName;
}
