using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "CommonRelic/FallenBanner")]
public class FallenBanner : CommonRelic
{
    [Header("Fallen Banner Level 1")]
    public float BannerDuration;
    public float MaxRange;
    public float BaseAttackIncrease;
    public EventReference BannerFallingSound;

    [Header("Fallen Banner Level 2")]
    public Vector3 BannerFollowPosition;
    public float BaseAttackIncreaseLevel2;
    public EventReference BannerSound;

    [Header("Fallen Banner Level 4")]
    public float BannerDurationLevel4;
    public int BannerMaxStacks;
}
