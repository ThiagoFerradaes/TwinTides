using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieTwoTreeManager : EnemyAttackPrefab
{
    ZombieTwoTree _info;
    Vector3 originalParentPosition;

    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieTwoTree;

        parentContext.Blackboard.IsAttacking = true;

        gameObject.SetActive(true);

        StartCoroutine(RainRoutine());
    }

    IEnumerator RainRoutine() {
        StartCoroutine(AttackDuration());

        originalParentPosition = parent.transform.position;

        parentContext.Anim.SetTrigger("IsAttacking");
        float enterAnimTimeout = 1f;
        float timer = 0f;

        while (parentContext.Anim.IsInTransition(0)) {
            yield return null;
            timer += Time.deltaTime;
            if (timer > enterAnimTimeout) {
                Debug.LogWarning("Transi��o para anima��o nunca come�ou.");
                break;
            }
        }

        timer = 0f;
        AnimatorStateInfo stateInfo = parentContext.Anim.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("Bombas")) {
            yield return null;
            timer += Time.deltaTime;
            stateInfo = parentContext.Anim.GetCurrentAnimatorStateInfo(0);
            if (timer > enterAnimTimeout) {
                Debug.LogWarning("Anima��o correta nunca entrou. Cancelando CryRoutine.");
                break;
            }
        }

        timer = 0f;
        while (stateInfo.normalizedTime <= 1) {
            yield return null;
            timer += Time.deltaTime;
            stateInfo = parentContext.Anim.GetCurrentAnimatorStateInfo(0);
            if (timer > enterAnimTimeout) {
                Debug.LogWarning("Anima��o correta nunca entrou. Cancelando CryRoutine.");
                break;
            }
        }

        for (int i =0; i < _info.amountOfbombs; i++) {
            InstantiateABomb();

            yield return new WaitForSeconds(_info.timeBetweenEachBomb);
        }

        End();
    }

    void InstantiateABomb() {
        Vector3 spawnPoint = Vector3.zero;
        bool foundValidPosition = false;
        int maxAttempts = 20;

        for (int attempt = 0; attempt < maxAttempts; attempt++) {
            Vector2 randomPoint = Random.insideUnitCircle * _info.rainRadius;
            Vector3 candidatePosition = parent.transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);
            candidatePosition.y = 0;

            if (NavMesh.SamplePosition(candidatePosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)) {
                spawnPoint = hit.position + Vector3.up * _info.bombHeight;
                foundValidPosition = true;
                break;
            }
        }

        if (foundValidPosition) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent, spawnPoint);
        }
        else {
            Vector3 centerPoint = new(originalParentPosition.x, _info.bombHeight, originalParentPosition.z);
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent, centerPoint);
        }

    }

    IEnumerator AttackDuration() {
        yield return new WaitForSeconds(_info.durationOfAttack);

        FakeEnding();
    }

    void FakeEnding() {

        parentContext.Blackboard.IsAttacking = false;

        parentContext.Blackboard.CanAttack = false;

        parentContext.Blackboard.Cooldowns[_info.ListOfAttacksNames[0]] = _info.rainCooldown;
    }
}
