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

    [Header("Minions")]
    public float CooldownForSearchEnemy;
    public float RangeToMel;

    [Header("Melee Minion")]
    public float MeleeMinionAttackRange;
    public float MeleMinionAttackCooldown;
    public float MeleeMinionDuration;
    public float MeleeMinionRangeToFindEnemy;
    public float MeleeAttackOffSet;
    public float MeleeAttackDuration;
    public float MeleeAttackDamage;

    [Header("Ranged Minion")]
    public float RangedMinionAttackRange;
    public float RangedMinionAttackCooldown;
    public float RangedMinionDuration;
    public float RangedMinionRangeToFindEnemy;
    public float RangedAttackSpeed;
    public float RangedAttackDuration;
    public float RangedAttackDamage;
}
