using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ZombieTwoOneManager : EnemyAttackPrefab {
    ZombieTwoOne _info;
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieTwoOne;

        gameObject.SetActive(true);

        parentContext.Blackboard.IsAttacking = true;

        if (parentContext.Blackboard.CurrentComboIndex < _info.comboNumberToUpgradeAttack) StartCoroutine(NormalPunchRoutine());
        else StartCoroutine(FinalAttack());
    }


    IEnumerator NormalPunchRoutine() {

        for(int i = 0; i < _info.quantidadeDeSocoPorCombo; i++) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent); 

            if (i < _info.quantidadeDeSocoPorCombo - 1) yield return new WaitForSeconds(_info.timeBetweenPunches);
        }

        parentContext.Blackboard.CurrentComboIndex++;

        EndOfAttack(_info.cooldownPunch);

        End();
    }

    IEnumerator FinalAttack() {

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent); // Primeiro soco mais forte

        yield return new WaitForSeconds(_info.timeBetweenFinalAttacks);

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 3, parent); // segundo soco mais forte

        yield return new WaitForSeconds(_info.durationOfFinalAttackPartTwo);

        parentContext.Blackboard.CurrentComboIndex = 1;

        EndOfAttack(_info.cooldownFinalAttack);

        End();
    }

    void EndOfAttack(float cooldown) {
        parentContext.Blackboard.IsAttacking = false;

        parentContext.Blackboard.CanAttack = false;

        parentContext.Blackboard.AttackCooldown = cooldown;
    }
}
