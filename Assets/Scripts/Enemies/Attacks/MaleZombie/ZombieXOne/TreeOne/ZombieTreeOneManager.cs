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

        parentContext.Anim.SetTrigger("IsAttacking");
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

        for (int i = 0; i < _info.amountOfPunchesPerCombo; i++) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent);

            if (i < _info.amountOfPunchesPerCombo - 1) yield return new WaitForSeconds(_info.timeBetweenPunches);
        }

        parentContext.Blackboard.CurrentComboIndex++;

        EndOfAttack(_info.cooldownPunch, _info.ListOfAttacksNames[0]);

        End();
    }

    IEnumerator FinalAttack() {

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent); // Spin

        yield return new WaitForSeconds(_info.totalSpinDuration + _info.timeBetweenSpinAndSmash);

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 3, parent); // segundo soco mais forte

        yield return new WaitForSeconds(_info.smashDuration);

        parentContext.Blackboard.CurrentComboIndex = 1;

        EndOfAttack(_info.smashCooldown, _info.ListOfAttacksNames[0]);

        End();
    }

    void EndOfAttack(float cooldown, string attackName) {
        parentContext.Blackboard.IsAttacking = false;

        parentContext.Blackboard.CanAttack = false;

        parentContext.Blackboard.Cooldowns[attackName] = cooldown;
    }
}
