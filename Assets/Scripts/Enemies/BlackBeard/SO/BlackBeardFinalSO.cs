using UnityEngine;

[CreateAssetMenu(menuName = "BlackBeardAttack/ Final")]
public class BlackBeardFinalSO : BlackBeardSO
{
    [Header("Attacks Prio")]
    public int AnchorPrio;
    public int BulletsPrio;
    public int BulletsRainPrio;
    public int CrossPrio;
    public int WavePrio;

    [Header("Attacks Cooldown")]
    public float AnchorCooldown;
    public float BulletsCooldown;
    public float BulletsRainCooldown;
    public float CrossCooldown;
    public float WaveCooldown;
    public float CooldownBetweenAttacks;

    [Header("Phase")]
    public float JumpToCenterPower;
    public float JumpToCenterDuration;
    public float DashToPositionSpeed;
}
