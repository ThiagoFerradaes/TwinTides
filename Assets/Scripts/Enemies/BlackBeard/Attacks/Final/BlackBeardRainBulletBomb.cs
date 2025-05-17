using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBeardRainBulletBomb : EnemyAttackPrefab
{
    BlackBeardBulletRainSO _info;
    bool collided;
    HealthManager _health;
    bool isStronger;

    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardBulletRainSO;

        if (_health == null) _health = parent.GetComponent<HealthManager>();

        isStronger = _health.ReturnCurrentHealth() < (_health.ReturnMaxHealth() / 2);

        SetPosition(position);
    }

    void SetPosition(Vector3 position) {
        transform.position = position;

        transform.localScale = Vector3.one * _info.BulletSize;

        gameObject.SetActive(true);

        StartCoroutine(FallRoutine());
    }

    IEnumerator FallRoutine() {
        while (!collided) {
            transform.position += _info.BulletFallSpeed * Time.deltaTime * -transform.up;
            yield return null;
        }

        collided = false;

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent, transform.position);

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Floor")) {

            if (collided) return;

            collided = true;
        }
    }
}
