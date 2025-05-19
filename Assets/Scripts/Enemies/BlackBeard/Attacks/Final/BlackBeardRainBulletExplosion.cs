using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBeardRainBulletExplosion : EnemyAttackPrefab
{
    BlackBeardBulletRainSO _info;
    float _bombIndex;

    public override void StartAttack(int enemyId, int skillId, Vector3 position, float bombIdenx) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardBulletRainSO;

        _bombIndex = bombIdenx;

        SetPosition(position);
    }

    void SetPosition(Vector3 position) {
        transform.position = position;

        float explosionRadius = _info.ExplosionRadius - (_info.ExplosionRadius * _bombIndex * _info.SecondaryExplosionRadiusPercent / 100);

        transform.localScale = Vector3.one * explosionRadius;

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.ExplosionDuration);

        if (_bombIndex == 0) EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 3, parent, transform.position);

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.DealDamage(_info.ExplosionDamage, true, true);
    }
}
