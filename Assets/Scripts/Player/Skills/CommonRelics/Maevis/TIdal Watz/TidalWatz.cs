using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "CommonRelic/TidalWatz")]
public class TidalWatz : CommonRelic
{
    [Header("Tidal Level 1")]
    public float CutSize;
    public int AmountOfCuts;
    public float Damage;
    public float CutDuration;
    public HealthDebuff BleedingDebuff;
    public EventReference CutSound;

    [Header("Tidal Level 2")]
    public float CutSizeLevel2;
    public int AmountOfCutsLevel2;
    public float CutInterval;

    [Header("Tidal Level 3")]
    public int AmountOfCutsLevel3;
    public float CutIntervalLevel3;
    public float CutDurationLevel3;
    public EventReference CutSoundFaster;

    [Header("Tidal Level 4")]
    public int AmountOfCutsLevel4;
    public Vector3 impactSize;
    public float BaseDamageImpact;
    public float ImpactDuration;
    public float ImpactOffset;
    [Range(0,100)]public float PercentOfDamageToAcumulate;
    public EventReference ImpactSound;
}
