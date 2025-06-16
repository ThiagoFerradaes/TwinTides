using FMODUnity;
using UnityEngine;

[CreateAssetMenu (menuName = "LegendaryRelic/Dreadfall")]
public class Dreadfall : LegendaryRelic
{
    [Header("Atributes")]
    public float ExplosionDuration;
    public float ExplosionRadius;
    public float ExplosionDamage;
    public float JumpDuration;
    public float JumpSpeed;
    public float JumpMaxRange;
    public float AmountOfShiled;
    public float ShieldDuration;
    public float FieldDuration;
    public float FieldDamagePerTick;
    public float FieldRadius;
    public float DamageCooldown;
    public HealthDebuff BleedDebuff;
    public EventReference JumpSound;
    public EventReference ImpactSound;
    public EventReference BurningAreaSound;

    [Header("Animação")]
    public string jumpAnimationName;
    public string impactAnimationName;
}
