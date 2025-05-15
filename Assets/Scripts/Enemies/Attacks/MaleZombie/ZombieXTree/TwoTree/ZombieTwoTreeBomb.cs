using System.Collections;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class ZombieTwoTreeBomb : EnemyAttackPrefab
{
    ZombieTwoTree _info;
    bool collided;

    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieTwoTree;

        parentContext.Blackboard.IsAttacking = true;

        SetPosition(position);
    }

    void SetPosition(Vector3 position) {
        transform.position = position;

        gameObject.SetActive(true);

        StartCoroutine(FallRoutine());
    }

    IEnumerator FallRoutine() {
        while (!collided) {
            transform.position += _info.fallSpeed * Time.deltaTime * -transform.up;
            yield return null;
        }

        collided = false;

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Floor")) return;

        if (collided) return;

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent, transform.position);

        collided = true;
    }
}
