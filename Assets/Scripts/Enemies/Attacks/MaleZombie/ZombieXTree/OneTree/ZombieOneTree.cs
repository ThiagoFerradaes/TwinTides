using UnityEngine;

[CreateAssetMenu (menuName = "EnemyAttack/ Zombie One Tree")]
public class ZombieOneTree : EnemyAttack
{
    [Header("ToxicCloud")]
    public float toxicCloudDuration;
    public float toxicCloudInicialSize;
    public float toxicCloudMaxSize;
    public float toxicCloudSizeIncreasePercent;
    public HealthDebuff toxicDebuff;
    public float toxicCloudCooldown;
    public float timeBetweenToxicAplication;
}
