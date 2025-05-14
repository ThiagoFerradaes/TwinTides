using UnityEngine;

[CreateAssetMenu(menuName = "BlackBeardAttack/ Runaway")]
public class BlackBeardRunawaySO : BlackBeardSO
{
    [Header("BlackBeardAtributes")]
    public float BlackBeardSpeed;
    public float BlackBeardStunTime;
    public float PhaseDuration;
    public float AmountOfHealthRecoveredPerEnemy;

    [Header("Enemies")]
    public float MaxAmountOfEnemies;
    public Vector2 ZombieOneOneRange;
    public Vector2 ZombieTwoOneRange;
    public Vector2 ZombieThreeOneRange;
    public Vector2 GirlOneOneRange;
    public Vector2 GirlTwoOneRange;
    public Vector2 GirlThreeOneRange;
    public Vector2 ZombieOneTwoRange;
    public Vector2 ZombieTwoTwoRange;
}
