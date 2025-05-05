using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAttack/ ZombieTwoOneAttack")]
public class ZombieTwoOne : EnemyAttack {

    [Header("Attack Atributes")]
    public float timeBetweenPunches;
    public float timeBetweenFinalAttacks;
    public int comboNumberToUpgradeAttack;
    public float cooldownPunch;
    public float cooldownFinalAttack;
    public int quantidadeDeSocoPorCombo;

    [Header("First Punch")]
    public float damageOfPunch;
    public float durationOfPunch;
    public float placementOfPunch;

    [Header("Final Attack")]
    public float damageOfFinalAttackPartOne;
    public float durationOfFinalAttackPartOne;
    public float placementOfFinalAttackPartOne;
    public float damageOfFinalAttackPartTwo;
    public float durationOfFinalAttackPartTwo;
    public float placementOfFinalAttackPartTwo;
    public float stunTimeFinalAttackTwo;
}
