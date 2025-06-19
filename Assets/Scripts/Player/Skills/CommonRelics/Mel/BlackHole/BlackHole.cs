using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "CommonRelic/BlackHole")]
public class BlackHole : CommonRelic
{
    [Header("Black Hole Level 1")]
    public Vector3 Size;
    public float Duration;
    public float Damage;
    [Range(0,100)]public float SlowPercent;
    public float MaxRange;
    public float DamageInterval;
    public EventReference BlackHoleSound;

    [Header("Black Hole Level 2")]
    public float StunInterval;
    public float StunDuration;

    [Header("Black Hole Level 3")]
    public float DurationLevel3;

    [Header("Black Hole Level 4")]
    public Vector3 SizeLevel4;
    public float StunDurationLevel4;
    [Range(0,100)]public float HealReductionPercent;

    [Header("Animation")]
    public string AnimationName;
    [Range(0, 1)] public float AnimationPercentToAttack;
}
