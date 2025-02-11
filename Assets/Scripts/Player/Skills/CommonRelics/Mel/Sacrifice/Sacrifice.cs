using UnityEngine;

[CreateAssetMenu(menuName ="CommonRelic/Sacrifice")]
public class Sacrifice : CommonRelic
{
    [Header("Atributes")]
    public float SphereSpeed;
    public float SphereBackSpeed;
    public float Duration;

    [Header("Scrifice Level 1")]
    [Range(0,100)]public float HealthLostPercent;
    [Range(0, 100)] public float HealthGainPercent;
    public float Healing;
    public int StackAmount;

    [Header("Scrifice Level 2")]
    [Range(0, 100)] public float HealthGainPercentLevel2;
    public float HealingLevel2;
    public int StackAmountLevel2;

    [Header("Scrifice Level 3")]
    [Range(0, 100)] public float HealthGainPercentLevel3;
    public HealthBuff HealingOverTimeBuff;

}
