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

    [Header("Melee Minion")]
    public float MeeleeMinionRange;
    public float MeeleeMinionAttackRange;
    public float MeeleMinionAttackCooldown;

    [Header("Ranged Minion")]
    public float RangedMinionRange;
    public float RangedMinionAttackRange;
    public float RangedMinionAttackCooldown;
}
