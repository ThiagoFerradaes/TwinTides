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

        parentContext.Anim.SetBool("IsAttacking", true);
        for (int i = 0; i < _info.quantidadeDeSocoPorCombo; i++) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent);

            if (i < _info.quantidadeDeSocoPorCombo - 1) yield return new WaitForSeconds(_info.timeBetweenPunches);
        }
        parentContext.Anim.SetBool("IsAttacking", false);

        parentContext.Blackboard.CurrentComboIndex++;

        EndOfAttack(_info.cooldownPunch, _info.ListOfAttacksNames[0]);

        End();
    }

    IEnumerator BetterPunch() {
        parentContext.Anim.SetTrigger("StrongAttack");
        float enterAnimTimeout = 1f;
        float timer = 0f;

        while (parentContext.Anim.IsInTransition(0)) {
            yield return null;
            timer += Time.deltaTime;
            if (timer > enterAnimTimeout) {
                Debug.LogWarning("Transição para animação nunca começou.");
                break;
            }
        }

        timer = 0f;
        AnimatorStateInfo stateInfo = parentContext.Anim.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("StrongAttack")) {
            yield return null;
            timer += Time.deltaTime;
            stateInfo = parentContext.Anim.GetCurrentAnimatorStateInfo(0);
            if (timer > enterAnimTimeout) {
                Debug.LogWarning("Animação correta nunca entrou. Cancelando CryRoutine.");
                break;
            }
        }

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent);

        yield return new WaitForSeconds(_info.durationOfBetterPunch);

        parentContext.Blackboard.CurrentComboIndex = 1;

        EndOfAttack(_info.cooldownBetterPunch, _info.ListOfAttacksNames[0]);

        End();
    }

    void EndOfAttack(float cooldown, string attackName) {

        parentContext.Blackboard.Cooldowns[attackName] = cooldown;
    }

    public override void End() {
        parentContext.Anim.SetBool("IsAttacking", false);

        parentContext.Blackboard.IsAttacking = false;

        parentContext.Blackboard.CanAttack = false;
        base.End();
    }
}
