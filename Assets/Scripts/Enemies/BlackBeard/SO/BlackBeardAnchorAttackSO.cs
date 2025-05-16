using UnityEngine;

[CreateAssetMenu(menuName = "BlackBeardAttack/ Anchor")]
public class BlackBeardAnchorAttackSO : BlackBeardSO
{
    [Header("Attack Over 50%")]
    public float AnchorRange;
    public float AnchorSpeed;
    public float AnchorDamage;
    public float AnchorStunTime;
    public float Cooldown;
    public float TimeBetweenAttacks;
    public Vector3 AnchorSize;
    public float AnchorOffset;

    [Header("Attack Under 50%")]
    public float AnchorRotationSPeed;
    public float ChainDamage;
    public float RotationDuration;
    public Vector2 ChainSize;
}
