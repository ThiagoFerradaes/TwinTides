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

        for (int i = 0; i < _info.quantidadeDeSocoPorCombo; i++) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent);

            if (i < _info.quantidadeDeSocoPorCombo - 1) yield return new WaitForSeconds(_info.timeBetweenPunches);
        }

        parentContext.Blackboard.CurrentComboIndex++;

        EndOfAttack(_info.cooldownPunch);

        End();
    }

    IEnumerator BetterPunch() {

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent);

        yield return new WaitForSeconds(_info.durationOfBetterPunch);

        parentContext.Blackboard.CurrentComboIndex = 1;

        EndOfAttack(_info.cooldownBetterPunch);

        End();
    }

    void EndOfAttack(float cooldown) {
        parentContext.Blackboard.IsAttacking = false;

        parentContext.Blackboard.CanAttack = false;

        parentContext.Blackboard.AttackCooldown = cooldown;
    }
}
