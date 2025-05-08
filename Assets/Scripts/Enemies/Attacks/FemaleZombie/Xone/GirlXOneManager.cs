using System.Collections;
using UnityEngine;

public class GirlXOneManager : EnemyAttackPrefab
{
    GirlXOne _info;

    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as GirlXOne;

        gameObject.SetActive(true);

        parentContext.Blackboard.IsAttacking = true;

        if (parentContext.Blackboard.CurrentComboIndex < _info.amountOfSequencesToShotStrongerBullet) StartCoroutine(BulletsRoutine());
        else StartCoroutine(StrongerBulletRoutine());
    }


    IEnumerator BulletsRoutine() {

        for (int i = 0; i < _info.amountOfBullets; i++) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent);

            if (i < _info.amountOfBullets - 1) yield return new WaitForSeconds(_info.timeBetweenBullets);
        }

        parentContext.Blackboard.CurrentComboIndex++;

        EndOfAttack(_info.cooldownOfBullets, _info.ListOfAttacksNames[0]);

        End();
    }

    IEnumerator StrongerBulletRoutine() {

        if (_info is GirlOneOne) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent);
        }

        else if (_info is GirlTwoOne girl) {
            for (int i = 0; i < girl.amountOfBulletsInFinalSequence; i++) {
                EnemySkillPooling.Instance.RequestInstantiateAttack(girl, 3, parent);

                yield return new WaitForSeconds(girl.timeBetweeenBulletsInFinalSequence);
            }
            yield return new WaitForSeconds(girl.timeBetweenSequenceAndStrongerBullet);

            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent);
        }

        parentContext.Blackboard.CurrentComboIndex = 1;

        EndOfAttack(_info.StrongerBulletCooldown, _info.ListOfAttacksNames[0]);

        End();
    }

    void EndOfAttack(float cooldown, string attackName) {
        parentContext.Blackboard.IsAttacking = false;

        parentContext.Blackboard.CanAttack = false;

        parentContext.Blackboard.Cooldowns[attackName] = cooldown;
    }
}
