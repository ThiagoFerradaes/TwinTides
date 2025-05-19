using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[CreateAssetMenu(menuName = "BlackBeardAttack/ Runaway")]
public class BlackBeardRunawaySO : BlackBeardSO
{
    [Header("BlackBeardAtributes")]
    public float BlackBeardSpeed;
    public float ArenaRadius;
    public float BlackBeardStunTime;
    public float PhaseDuration;
    public float AmountOfHealthRecoveredPerEnemy;
    public float JumpPower;
    public float JumpDuration;

    [Header("Enemies")]
    public List<GroupOfEnemies> ListOfGroups;
}
