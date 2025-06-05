using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAttack/ Girl Tree One")]
public class GirlTreeOne : GirlTwoOne
{

    [Header("Explosion")]
    public float ExplosionSize;
    public float ExplosionDamage;
    public float ExplosionDuration;
    public EventReference ExplosionSound;
}
