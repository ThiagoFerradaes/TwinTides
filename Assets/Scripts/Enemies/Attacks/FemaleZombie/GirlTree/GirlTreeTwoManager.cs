using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GirlTreeTwoManager : EnemyAttackPrefab {
    GirlTreeTwo _info;

    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as GirlTreeTwo;

        gameObject.SetActive(true);

        parentContext.Blackboard.IsAttacking = true;

        StartCoroutine(BombsRoutine());
    }

    IEnumerator BombsRoutine() {

        for (int i = 0; i < _info.amountOfBombs; i++) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent);

            if (i < _info.amountOfBombs - 1) yield return new WaitForSeconds(_info.timeBetweenBombs);
        }

        EndOfAttack(_info.cooldown, _info.ListOfAttacksNames[0]);


        parentContext.Blackboard.CurrentComboIndex++;

        End();
    }


    void EndOfAttack(float cooldown, string attackName) {
        parentContext.Blackboard.IsAttacking = false;

        parentContext.Blackboard.CanAttack = false;

        parentContext.Blackboard.Cooldowns[attackName] = cooldown;
    }
}
