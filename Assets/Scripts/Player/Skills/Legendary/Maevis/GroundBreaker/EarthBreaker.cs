using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "LegendaryRelic/EarthBreaker")]
public class EarthBreaker : LegendaryRelic
{
    [Header("Atributes")]
    public float AmountOfImpacts;
    public float CooldownBetweenEachImpact;
    public float ImpactDuration;
    [Range(0,100)]public float ImpactGrowthPercent;
    public float Damage;
    public float StunDuration;
    public Vector3 InicialImpactSize;
    public EventReference EarthImpactSound;

    [Header("Animation")]
    public string animationName;
    
}
