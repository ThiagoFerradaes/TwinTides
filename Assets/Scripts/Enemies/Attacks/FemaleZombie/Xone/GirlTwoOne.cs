using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAttack/ Girl Two One")]
public class GirlTwoOne : GirlXOne
{
    [Header("FinalSequenceShooting")]
    public float amountOfBulletsInFinalSequence;
    public float timeBetweeenBulletsInFinalSequence;
    public float finalSequenceBulletsDamage;
    public float finalSequenceBulletsSpeed;
    public float finalSequenceBulletsRange;
    public float timeBetweenSequenceAndStrongerBullet;
    public EventReference SequenceBulletShootSound;
}
