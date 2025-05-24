using UnityEngine;

public class BlackBeardAttackPrefab : EnemyAttackPrefab
{
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        parent.GetComponent<BlackBeardMachineState>().OnChangedPhase -= End;
        parent.GetComponent<BlackBeardMachineState>().OnChangedPhase += End;
    }
    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        parent.GetComponent<BlackBeardMachineState>().OnChangedPhase -= End;
        parent.GetComponent<BlackBeardMachineState>().OnChangedPhase += End;
    }
    public override void StartAttack(int enemyId, int skillId, Vector3 position, float number) {
        base.StartAttack(enemyId, skillId);

        parent.GetComponent<BlackBeardMachineState>().OnChangedPhase -= End;
        parent.GetComponent<BlackBeardMachineState>().OnChangedPhase += End;
    }

    public override void End() {
        parent.GetComponent<BlackBeardMachineState>().OnChangedPhase -= End;
        base.End();
    }
}
