using System.Collections;
using UnityEngine;
using UnityEngine.TerrainTools;

public class ZombieTreeOneManager : EnemyAttackPrefab
{
    ZombieTreeOne _info;
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieTreeOne;
        gameObject.SetActive(true);

        parentContext.Blackboard.IsAttacking = true;

        if (parentContext.Blackboard.CurrentComboIndex < _info.comboNumberToUpgradeAttack) StartCoroutine(NormalPunchRoutine());
        else StartCoroutine(FinalAttack());
    }


    IEnumerator NormalPunchRoutine() {

        for (int i = 0; i < _info.amountOfPunchesPerCombo; i++) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent);

            if (i < _info.amountOfPunchesPerCombo - 1) yield return new WaitForSeconds(_info.timeBetweenPunches);
        }

        parentContext.Blackboard.CurrentComboIndex++;

        EndOfAttack(_info.cooldownPunch);

        End();
    }

    IEnumerator FinalAttack() {

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent); // Spin

        yield return new WaitForSeconds(_info.totalSpinDuration + _info.timeBetweenSpinAndSmash);

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 3, parent); // segundo soco mais forte

        yield return new WaitForSeconds(_info.smashDuration);

        parentContext.Blackboard.CurrentComboIndex = 1;

        EndOfAttack(_info.smashCooldown);

        End();
    }

    void EndOfAttack(float cooldown) {
        parentContext.Blackboard.IsAttacking = false;

        parentContext.Blackboard.CanAttack = false;

        parentContext.Blackboard.AttackCooldown = cooldown;
    }
}
