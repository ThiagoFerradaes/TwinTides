using System;
using System.Collections;
using UnityEngine;

public class ZombieOneTwoExplosion : EnemyAttackPrefab
{
    ZombieOneTwo _info;

    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieOneTwo;

        SetPosition();
    }

    private void SetPosition() {

        transform.localScale = _info.jumpExplosionRadius * Vector3.one;

        transform.position = parent.transform.position;

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.jumpExplosionDuration);

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.DealDamage(_info.jumpDamage, true, true);
    }
}
