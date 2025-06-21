using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "LegendaryRelic/SpiritConvergence")]
public class SpiritConvergence : LegendaryRelic
{
    [Header("Invocation")]
    public float SkillDuration;
    public float MeleeMinionCooldown;
    public float RangedMinionCooldown;
    public float AmountOfTimesDurationCanExtend;
    public float DurationExtensionTime;
    public EventReference InvocationSound;

    [Header("Minions")]
    public float CooldownForSearchEnemy;
    public float RangeToMel;
    public float MeleeMinionSpeed;
    public float RangedMinionSpeed;

    [Header("Melee Minion")]
    public float MeleeMinionAttackRange;
    public float MeleeMinionAttackCooldown;
    public float MeleeMinionDuration;
    public float MeleeMinionRangeToFindEnemy;
    public float MeleeAttackOffSet;
    public float MeleeAttackDuration;
    public float MeleeAttackDamage;
    public EventReference MeleeMinionHitSound;

    [Header("Ranged Minion")]
    public float RangedMinionAttackRange;
    public float RangedMinionAttackCooldown;
    public float RangedMinionDuration;
    public float RangedMinionRangeToFindEnemy;
    public float RangedAttackSpeed;
    public float RangedAttackDuration;
    public float RangedAttackDamage;
    public EventReference RangedMinionHitSound;

    [Header("Animation")]
    public string AnimationName;
    [Range(0, 1)] public float AnimationPercentToAttack;
}
