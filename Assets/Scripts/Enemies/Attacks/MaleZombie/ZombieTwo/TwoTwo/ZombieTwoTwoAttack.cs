using System.Collections;
using UnityEngine;

public class ZombieTwoTwoAttack : EnemyAttackPrefab {
    ZombieTwoTwo _info;
    bool playerColision;

    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieTwoTwo;

        transform.localScale = new Vector3(_info.hitBoxSize, 0.1f, _info.hitBoxSize);

        if (transform.parent == null) {
            transform.SetParent(parent.transform);
        }

        transform.localPosition = Vector3.zero;

        gameObject.SetActive(true);

        parentContext.Blackboard.IsAttacking = true;

        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine() {

        Vector3 direction = (parentContext.Blackboard.Target.position - parent.transform.position).normalized;

        float dashDuration = _info.dashDistance / _info.dashSpeed;

        float timer = 0;

        while (timer < dashDuration && !playerColision) {
            parent.transform.position += (_info.dashSpeed * Time.deltaTime * direction);
            timer += Time.deltaTime;
            yield return null;
        }

        AttackEnd();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        playerColision = true;

        health.DealDamage(_info.attackDamage, true, true);

        if (!other.TryGetComponent<MovementManager>(out MovementManager movement)) return;

        movement.StunWithTime(_info.stunTime);
    }

    void AttackEnd() {
        playerColision = false;

        parentContext.Blackboard.IsAttacking = false;

        parentContext.Blackboard.CanAttack = false;

        parentContext.Blackboard.Cooldowns[_info.ListOfAttacksNames[0]] = _info.attackCooldown;

        End();
    }
}
