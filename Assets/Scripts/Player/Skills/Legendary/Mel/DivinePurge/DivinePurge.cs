using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "LegendaryRelic/DivinePurge")]
public class DivinePurge : LegendaryRelic {

    [Header("Atributes")]
    public Vector3 SkillSize;
    public float Duration;
    public float DamagePerTick;
    public float DamageCooldown;
    public float AmountOfHealToMaevis;
    [Range(0, 100)] public float PercentOfHealingBasedOnDamage;
    public float ZOffSett;
    public EventReference LaserSound;

    [Header("Animation")]
    public string animationName;
}
