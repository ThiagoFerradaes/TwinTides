using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "CommonRelic/WardStone")]
public class WardStone : CommonRelic {

    [Header("WardStone Level 1")]
    public Vector3 ExplosionRadius;
    public HealthBuff Debuffblocker;
    public float ExplosionDuration;
    public EventReference ExplosionSound;

    [Header("WardStone Level 2")]
    public HealthBuff HealingIncreaseBuff;
    public HealthBuff ShieldIncreaseBuff;
    public float AmountOfShield;
    public float ShieldDuration;

    [Header("WardStone Level 3")]
    public Vector3 ExplosionRadiusLevel3;
    public float AreaDuration;

    [Header("WardStone Level 4")]
    public HealthBuff BuffblockerLevel4;
    public HealthBuff HealingIncreaseBuffLevel4;
    public HealthBuff ShieldIncreaseBuffLevel4;
    public float AmountOfHealing;
    public float HealingInterval;
    [Range(0,100)]public float PercentOfShieldFromExtraHealing;
    public float AreaDurationLevel4;
    public float ExtraShieldDuration;
    public EventReference AreaSound;

    [Header("Animation")]
    public string AnimationName;
    [Range(0, 1)] public float AnimationPercentToAttack;
}
