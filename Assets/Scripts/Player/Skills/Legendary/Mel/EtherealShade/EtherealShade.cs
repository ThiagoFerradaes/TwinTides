using FMODUnity;
using UnityEngine;

[CreateAssetMenu (menuName = "LegendaryRelic/EhterealShade")]
public class EtherealShade : LegendaryRelic
{
    [Header("Atributos")]
    public float TotemDuration;
    public float GrowthPercentage;
    public float HealCooldown;
    public float MaxAmountOfGrowths;
    public float Heal;
    public float BaseDamage;
    public float PercentOfDamageIncreasePerGrowth;
    public float MaxRangeToPlace;
    public float ExplosionDuration;
    public float InicialRadius;
    public float HealingEffectDuration;
    public EventReference GrowthSound;
    public EventReference DamageExplosionSound;
    public EventReference HealExplosionSound;
    public EventReference PositionSound;

    [Header("Animation")]
    public string AnimationName;
    [Range(0, 1)] public float AnimationPercentToAttack;
}
