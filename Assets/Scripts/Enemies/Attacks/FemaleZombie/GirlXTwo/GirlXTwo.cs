using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAttack/ Girl One Two")]
public class GirlXTwo : EnemyAttack
{
    [Header("Healing Area")]
    public float amountOfHealing;
    public float healingDuration;
    public float healingSize;
    public float timeBetweenHeals;
    public float amountOfShield;
    public float durationOfShield;
    public float cooldown;
    public EventReference HealingAreaSound;
}
