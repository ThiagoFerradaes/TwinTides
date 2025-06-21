using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "CommonRelic/WarCry")]
public class Warcry : CommonRelic {
    [Header("Warcry Level 1")]
    public float Duration;
    [Range(0, 100)] public float PercentAttackSpeed;
    public float ExplosionRadius;
    public float ExplosionDuration;
    public EventReference CrySound;

    [Header("Warcry Level 2")]
    [Range(0, 200)] public float PercentAttackSpeedLevel2;

    [Header("Warcry Level 3")]
    public float StunDuration;
    [Range(0, 200)] public float PercentMoveSpeedGain;
    public float ExplosionRadiusLevel3;

    [Header("Warcry Level 4")]
    public AttackSkill NormalMaevisAttack;
    public AttackSkill EnhancedMaevisAttack;
    [Range(0,200)]public float PercentAttackSpeedLevel4;
    public float DurationLevel4;

    [Header("Animation")]
    public string AnimationName;
    [Range(0,1)]public float AnimationPercentToAttack;
}
