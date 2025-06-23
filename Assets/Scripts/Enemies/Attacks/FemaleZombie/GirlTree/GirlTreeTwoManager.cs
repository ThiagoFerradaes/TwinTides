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

            float enterAnimTimeout = 1f;
            float timer = 0f;

            while (anim.IsInTransition(0)) {
                yield return null;
                timer += Time.deltaTime;
                if (timer > enterAnimTimeout) {
                    Debug.LogWarning("Transição para animação nunca começou.");
                    break;
                }
            }

            timer = 0f;
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            while (!stateInfo.IsName("Tiro")) {
                yield return null;
                timer += Time.deltaTime;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                if (timer > enterAnimTimeout) {
                    Debug.LogWarning("Animação correta nunca entrou. Cancelando CryRoutine.");
                    break;
                }
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
