using System.Collections;
using UnityEngine;

public class GirlXOneManager : EnemyAttackPrefab
{
    GirlXOne _info;
    Animator anim;

    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as GirlXOne;

        gameObject.SetActive(true);

        parentContext.Blackboard.IsAttacking = true;
        if (anim == null) anim = parentContext.Anim;

        if (parentContext.Blackboard.CurrentComboIndex < _info.amountOfSequencesToShotStrongerBullet) StartCoroutine(BulletsRoutine());
        else StartCoroutine(StrongerBulletRoutine());
    }


    IEnumerator BulletsRoutine() {
        anim.SetBool("IsAttackingBool", true);
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        while (anim.IsInTransition(0)) yield return null;

        while (stateInfo.IsName("Shoot") == false) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        for (int i = 0; i < _info.amountOfBullets; i++) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent);

            if (i < _info.amountOfBullets - 1) yield return new WaitForSeconds(_info.timeBetweenBullets);
        }
        anim.SetBool("IsAttackingBool", false);
        parentContext.Blackboard.CurrentComboIndex++;

        EndOfAttack(_info.cooldownOfBullets, _info.ListOfAttacksNames[0]);

        End();
    }

    IEnumerator StrongerBulletRoutine() {

        if (_info is GirlOneOne) {
            anim.SetTrigger("StrongAttack");
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
            while (!stateInfo.IsName("Strong Attack")) {
                yield return null;
                timer += Time.deltaTime;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                if (timer > enterAnimTimeout) {
                    Debug.LogWarning("Animação correta nunca entrou. Cancelando CryRoutine.");
                    break;
                }
            }

            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent);
        }

        else if (_info is GirlTwoOne girl) {
            // Ataques Rapidos
            anim.SetBool("TiroRapido", true);
            for (int i = 0; i < girl.amountOfBulletsInFinalSequence; i++) {
                EnemySkillPooling.Instance.RequestInstantiateAttack(girl, 3, parent);

                yield return new WaitForSeconds(girl.timeBetweeenBulletsInFinalSequence);
            }
            anim.SetBool("TiroRapido", false);
            
            // Ataque mais forte
            anim.SetTrigger("StrongAttack");
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
            while (!stateInfo.IsName("Strong Attack")) {
                yield return null;
                timer += Time.deltaTime;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                if (timer > enterAnimTimeout) {
                    Debug.LogWarning("Animação correta nunca entrou. Cancelando CryRoutine.");
                    break;
                }
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
