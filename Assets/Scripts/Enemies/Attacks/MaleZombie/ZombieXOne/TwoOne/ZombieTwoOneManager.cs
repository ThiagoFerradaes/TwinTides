using System.Collections;
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

        for(int i = 0; i < _info.amountOfPunchesPerCombo; i++) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent);
            Debug.Log("Instantiate punch");
            if (i < _info.amountOfPunchesPerCombo - 1) yield return new WaitForSeconds(_info.timeBetweenPunches);
        }

        parentContext.Blackboard.CurrentComboIndex++;

        EndOfAttack(_info.cooldownPunch, _info.ListOfAttacksNames[0]);

        End();
    }

    IEnumerator FinalAttack() {

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent); // Primeiro soco mais forte

        yield return new WaitForSeconds(_info.timeBetweenFinalAttacks);

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 3, parent); // segundo soco mais forte

        yield return new WaitForSeconds(_info.durationOfFinalAttackPartTwo);

        parentContext.Blackboard.CurrentComboIndex = 1;

        EndOfAttack(_info.cooldownFinalAttack, _info.ListOfAttacksNames[0]);

        End();
    }

    void EndOfAttack(float cooldown, string attackName) {
        parentContext.Blackboard.IsAttacking = false;

        parentContext.Blackboard.CanAttack = false;

        parentContext.Blackboard.Cooldowns[attackName] = cooldown;
    }
}
