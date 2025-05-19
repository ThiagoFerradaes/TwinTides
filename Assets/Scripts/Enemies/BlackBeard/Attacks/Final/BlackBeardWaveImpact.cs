using System.Collections;
using UnityEngine;

public class BlackBeardWaveImpact : EnemyAttackPrefab
{
    BlackBeardWaveAttackSO _info;
    Vector3 pos;
    HealthManager _health;
    bool isStronger;
    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        if (_info == null) _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardWaveAttackSO;

        if (_health == null) _health = parent.GetComponent<HealthManager>();

        pos = position;

        isStronger = _health.ReturnCurrentHealth() < (_health.ReturnMaxHealth() / 2);

        DefinePosition();

    }

    private void DefinePosition() {
        transform.localScale = Vector3.one * _info.WaveInitialRadius;

        transform.position = pos;

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        float speed = isStronger ? _info.WaveSpeedStronger : _info.WaveSpeed;
        float duration = _info.WaveMaxRadius / speed;

        float timer = 0;

        while (timer < duration) {
            timer += Time.deltaTime;
            float currentRadius = _info.WaveInitialRadius + timer * speed;
            transform.localScale = Vector3.one * currentRadius;
            yield return null;
        }

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.DealDamage(_info.WaveDamage, true, true);

        if (isStronger) {
            _health.ApplyShield(_info.AmountOfShieldGainPerWaveHit, _info.ShieldDuration, true);
        }
    }
}
