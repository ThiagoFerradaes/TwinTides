using System.Collections;
using UnityEngine;

public class ZombieOneOneManager : EnemyAttackPrefab {

    ZombieOneOne _info;
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieOneOne;

        gameObject.SetActive(true);

        parentContext.Blackboard.IsAttacking = true;

        if (parentContext.Blackboard.CurrentComboIndex < _info.comboNumberToUpgradeAttack) StartCoroutine(NormalPunchRoutine());
        else StartCoroutine(BetterPunch());
    }


    IEnumerator NormalPunchRoutine() {

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent);

        yield return new WaitForSeconds(_info.timeBetweenPunches);

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent);

        parentContext.Blackboard.CurrentComboIndex++;

        parentContext.Blackboard.IsAttacking = false;

        End();
    }

    IEnumerator BetterPunch() {

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent);

        yield return new WaitForSeconds(_info.durationOfBetterPunch);

        parentContext.Blackboard.CurrentComboIndex = 1;

        parentContext.Blackboard.IsAttacking = false;

        End();
    }
}
