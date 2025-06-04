using FMODUnity;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu (menuName = "EnemyAttack/ ZombieOneOneAttack")]
public class ZombieOneOne : EnemyAttack {

    [Header("Attack Atributes")]
    public float timeBetweenPunches;
    public int comboNumberToUpgradeAttack;
    public float cooldownPunch;
    public float cooldownBetterPunch;
    public int quantidadeDeSocoPorCombo;

    [Header("First Punch")]
    public float damageOfPunch;
    public float durationOfPunch;
    public float placementOfPunch;
    public EventReference NormalPunchSound;

    [Header("Better Punch")]
    public float damageOfBetterPunch;
    public float durationOfBetterPunch;
    public float placementOfBetterPunch;
    public EventReference BetterPunchSound;
}
