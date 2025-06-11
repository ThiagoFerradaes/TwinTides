using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAttack/ Girl One Three")]
public class GirlOneThree : EnemyAttack
{
    [Header("BasicPuddle")]
    public float puddleDamage;
    public float puddleDuration;
    public float puddleSize;
    public float puddleSizeIncreasePercent;
    public float puddleMaxSize;
    public float puddleDurationIncreasePercent;
    public float puddleMaxDuration;
    public float timeBetweenDamages;
    public float puddleOffSett;
    public float cooldown;
    public EventReference PuddleSound;
}
