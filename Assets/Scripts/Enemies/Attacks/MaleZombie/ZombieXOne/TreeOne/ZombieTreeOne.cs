using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAttack/ ZombieTreeOneAttack")]
public class ZombieTreeOne : EnemyAttack
{
    [Header("AttackAtributes")]
    public int comboNumberToUpgradeAttack;

    [Header("Punch")]
    public float timeBetweenPunches;
    public float cooldownPunch;
    public int amountOfPunchesPerCombo;
    public float damagePerPunch;
    public float durationOfPunch;
    public float placementOfPunch;
    public EventReference NormalPunchSound;

    [Header("Tentacle Spin")]
    public float amountOfTicksPerSpin;
    public float totalSpinDuration;
    public float damagePerTick;
    public float timeBetweenSpinAndSmash;
    public EventReference TentacleSpinSound;

    [Header("Smash")]
    public float smashDamage;
    public float smashDuration;
    public float smashPlacement;
    public float smashStunTime;
    public float smashCooldown;
    public EventReference SmashSound;
}
