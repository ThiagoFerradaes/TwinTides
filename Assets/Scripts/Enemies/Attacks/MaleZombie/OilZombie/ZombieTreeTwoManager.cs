using System.Collections;
using UnityEngine;
using UnityEngine.TerrainTools;

public class ZombieTreeTwoManager : EnemyAttackPrefab
{
    ZombieTreeTwo _info;
    Transform target;

    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieTreeTwo;

        target = parentContext.Blackboard.Target;

        gameObject.SetActive(true);

        parentContext.Blackboard.IsAttacking = true;

        if (parentContext.Blackboard.Cooldowns[_info.ListOfAttacksNames[1]] <= 0) StartCoroutine(OilRoutine());
        else StartCoroutine(PunchRoutine());
    }

    IEnumerator PunchRoutine() {

        parentContext.MManager.IncreaseMoveSpeed(_info.increaseInMoveSpeedToPunch);

        while (Vector3.Distance(parent.transform.position, target.position) > _info.distanceToPunch) {
            parentContext.Agent.SetDestination(target.position);
            parentContext.Agent.speed = parentContext.MManager.ReturnMoveSpeed();
            yield return null;
        }

        parentContext.Agent.ResetPath();
        parentContext.Agent.velocity = Vector3.zero;

        Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);
        while (Quaternion.Angle(transform.rotation, lookRotation) > 5f) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, _info.rootationSpeedToPunch * 360 * Time.deltaTime);
            yield return null;
        }

        parentContext.MManager.DecreaseMoveSpeed(_info.increaseInMoveSpeedToPunch);

        for (int i = 0; i < _info.amountOfPunches; i++) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent);

            if (i < _info.amountOfPunches - 1) yield return new WaitForSeconds(_info.timeBetweenPunches);
        }

        EndOfAttack(_info.punchCooldown, _info.ListOfAttacksNames[0]);

        End();
    }

    IEnumerator OilRoutine() {

        yield return null; // Aqui pode ter alguma animação etc

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent, target.position);

        EndOfAttack(_info.oilCooldown, _info.ListOfAttacksNames[1]);

        End();
    }

    void EndOfAttack(float cooldown, string attackName) {
        parentContext.Blackboard.IsAttacking = false;

        parentContext.Blackboard.CanAttack = false;

        parentContext.Blackboard.Cooldowns[attackName] = cooldown;
    }
}
