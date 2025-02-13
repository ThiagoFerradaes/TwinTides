using UnityEngine;

[CreateAssetMenu(menuName = "CommonRelic/TidalWatz")]
public class TidalWatz : CommonRelic
{
    [Header("Tidal Level 1")]
    public Vector3 CutSize;
    public Vector3 CutPosition;
    public int AmountOfCuts;
    public float Damage;
    public float CutDuration;
    public HealthDebuff BleedingDebuff;

    [Header("Tidal Level 2")]
    public Vector3 CutSizeLevel2;
    public Vector3 CutPositionLevel2;
    public int AmountOfCutsLevel2;
    public float CutInterval;

    [Header("Tidal Level 3")]
    public int AmountOfCutsLevel3;
    public float CutIntervalLevel3;
    public float CutDurationLevel3;

    [Header("Tidal Level 4")]
    public int AmountOfCutsLevel4;
    public float BaseDamageImpact;
    public float ImpactDuration;
    public float ImpactOffset;
    [Range(0,100)]public float PercentOfDamageToAcumulate;
}
