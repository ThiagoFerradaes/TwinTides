using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "BlackBeardAttack/ Bullets Attack")]
public class BlackBeardBulletsAttackSO : BlackBeardSO
{
    [Header("Bullets Atributes")]
    public float Duration;
    public float DashSpeed;
    public int AmountOfTimesAttack;
    public float Damage;
    public float TimeBetweenOneAttackAndTheNext;
    public float TimeBetweenDamages;
    public float BulletsSize;
    public EventReference DashSound;

    [Header("Bullets Atributes Stronger")]
    public float DurationStronger;
    public float DashSpeedStronger;
    public int AmountOfAttacksStronger;
    public float BulletSizeStronger;
    public EventReference BulletsSound;

    [Header("Animation")]
    public string AnimationTriggerName;
}
