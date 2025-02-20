using UnityEngine;

[CreateAssetMenu(menuName = "CommonRelic/FallenBanner")]
public class FallenBanner : CommonRelic
{
    [Header("Fallen Banner Level 1")]
    public float BannerDuration;
    public float MaxRange;

    [Header("Fallen Banner Level 2")]
    public Vector3 BannerFollowPosition;

    [Header("Fallen Banner Level 4")]
    public float BannerDurationLevel4;
    public int BannerMaxStacks;
}
