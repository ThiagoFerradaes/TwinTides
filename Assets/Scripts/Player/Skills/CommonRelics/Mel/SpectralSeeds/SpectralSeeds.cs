using FMODUnity;
using UnityEngine;

[CreateAssetMenu (menuName = "CommonRelic/SpectralSeeds")]
public class SpectralSeeds : CommonRelic
{
    [Header("Atributes")]
    public Vector3 RingPosition;
    public float SeedRadius;
    public float SeedInicialPosition;
    public Vector3 RingSize;
    public Vector3 SeedSize;
    public float SeedSpeed;
    public float RingRotationDuration;

    [Header("Spectral Seeds Level 1")]
    public int AmountOfSeeds;
    public float Duration;
    public float ExplosionRadius;
    public float ExplosionDuration;
    public float Damage;
    public EventReference ExplosionSound;

    [Header("Spectral Seeds Level 2")]
    public int AmountOfSeedsLevel2;
    public float ExplosionRadiusLevel2;

    [Header("Spectral Seeds Level 3")]
    public int AmountOfSeedsLevel3;
    public float AmountOfHealToMaevis;
    [Range(0,100)]public float PercentOfDamageToHeal;

    [Header("Spectral Seeds Level 4")]
    public float DurationLevel4;
    public float ExplosionsInterval;

    [Header("Animation")]
    public string AnimationName;
    [Range(0, 1)] public float AnimationPercentToAttack;
}
