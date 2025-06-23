using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GirlTreeTwoManager : EnemyAttackPrefab {
    GirlTreeTwo _info;
    Animator anim;
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as GirlTreeTwo;

        if (anim == null) anim = parentContext.Anim;

        gameObject.SetActive(true);

        parentContext.Blackboard.IsAttacking = true;

        StartCoroutine(BombsRoutine());
    }

    IEnumerator BombsRoutine() {

        for (int i = 0; i < _info.amountOfBombs; i++) {

            anim.SetTrigger("IsAttacking");

            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            while (anim.IsInTransition(0)) yield return null;

            while (stateInfo.IsName("Tiro") == false) {
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            }

            // Espera a animação terminar
            while (stateInfo.normalizedTime < 0.5f) {
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            }

            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent);

            if (i < _info.amountOfBombs - 1) {
                while (stateInfo.normalizedTime < 1f && stateInfo.IsName("Tiro")) {
                    yield return null;
                    stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                }

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
}
