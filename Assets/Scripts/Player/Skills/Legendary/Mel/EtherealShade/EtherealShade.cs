using UnityEngine;

[CreateAssetMenu (menuName = "LegendaryRelic/EhterealShade")]
public class EtherealShade : LegendaryRelic
{
    [Header("Atributos")]
    public float CloneDuration;
    public float GrowthPercentage;
    public float HealCooldown;
    public float MaxAmountOfGrowths;
    public float Heal;
    public float BaseDamage;
    public float PercentOfDamageIncreasePerGrowth;
    public float MaxRangeToPlace;
    public float ExplosionDuration;
    public float InicialRadius;
}
