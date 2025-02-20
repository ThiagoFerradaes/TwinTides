using UnityEngine;

[CreateAssetMenu(menuName = "CommonRelic/WarCry")]
public class Warcry : CommonRelic {
    [Header("Warcry Level 1")]
    public float Duration;
    [Range(0, 100)] public float PercentAttackSpeed;
    public float ExplosionRadius;
    public float ExplosionDuration;

    [Header("Warcry Level 2")]
    [Range(0, 100)] public float PercentAttackSpeedLevel2;

    [Header("Warcry Level 3")]
    public float StunDuration;
    [Range(0, 100)] public float PercentMoveSpeedGain;
    public float ExplosionRadiusLevel3;

    [Header("Warcry Level 4")]
    public AttackSkill NormalMaevisAttack;
    public AttackSkill EnhancedMaevisAttack;
    public float PercentAttackSpeedLevel4;
    public float DurationLevel4;
}
