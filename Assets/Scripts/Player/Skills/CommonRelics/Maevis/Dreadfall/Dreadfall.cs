using UnityEngine;

[CreateAssetMenu (menuName = "CommonRelic/Dreadfall")]
public class Dreadfall : CommonRelic
{
    [Header("Dreadfall Level 1")]
    public float ExplosionRadius;
    public float ExplosionDamage;
    public float ExplosionDuration;
    public float JumpDuration;
    public float JumpSpeed;
    public float JumpMaxRange;

    [Header("Dreadfall Level 2")]
    public float AmountOfShiled;
    public float ShieldDuration;

    [Header("Dreadfall Level 3")]
    public float ExplosionRadiusLevel3;
    public float ExplosionDamageLevel3;

    [Header("Dreadfall Level 4")]
    public float FieldDuration;
    public float FieldDamagePerTick;
    public float DamageCooldown;
    public HealthDebuff BleedDebuff;
}
