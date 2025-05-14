using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAttack/ Zombie Tree Two")]
public class ZombieTreeTwo : EnemyAttack
{
    [Header("Oil")]
    public float oilSize;
    public float oilDuration;
    public float oilSpeedReduction;
    public float oilCooldown;
    public float oilDamage;
    public float oilDamageInterval;
    public Material oilNormalMaterial;
    public Material oilBurningMaterial;

    [Header("Punches")]
    public float distanceToPunch;
    public float increaseInMoveSpeedToPunch;
    public float amountOfPunches;
    public float punchDamage;
    public float punchDuration;
    public float punchPlacement;
    public float timeBetweenPunches;
    public float punchCooldown;
    public float rootationSpeedToPunch;
}
