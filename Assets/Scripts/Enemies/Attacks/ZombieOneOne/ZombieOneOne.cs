using System.Collections;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu (menuName = "EnemyAttack/ ZombieOneOneAttack")]
public class ZombieOneOne : EnemyAttack {

    [Header("Attack Atributes")]
    public float timeBetweenPunches;
    public int comboNumberToUpgradeAttack;

    [Header("First Punch")]
    public float damageOfPunch;
    public float durationOfPunch;
    public float placementOfPunch;

    [Header("Better Punch")]
    public float damageOfBetterPunch;
    public float durationOfBetterPunch;
    public float placementOfBetterPunch;
}
