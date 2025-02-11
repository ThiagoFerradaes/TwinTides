using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "CommonRelic/PhantomAura")]
public class PhantomAura : CommonRelic
{
    [Header("Aura Level 1")]
    public Vector3 AuraSize;
    public float Duration;
    public float Damage;
    public float DamageInterval;

    [Header("Aura Level 2")]
    [Range(0,100)]public float HealingPercent;

    [Header("Aura Level 4")]
    public Vector3 AuraSizeLevel4;
    public float DurationLevel4;
    public float DamageLevel4;
}
