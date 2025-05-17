using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBeardBulletRainManager : EnemyAttackPrefab {
    BlackBeardBulletRainSO _info;
    Vector3 pos;
    HealthManager _health;
    bool isStronger;
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        if (_info == null) _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardBulletRainSO;

        if (_health == null) _health = parent.GetComponent<HealthManager>();

        isStronger = _health.ReturnCurrentHealth() < (_health.ReturnMaxHealth() / 2);

        DefinePosition();
    }

    private void DefinePosition() {

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        int amountOfBullets = isStronger ? _info.AmountOfBulletsStronger : _info.AmountOfBullets;
        float radius = isStronger ? _info.RadiusStronger : _info.Radius;

        Transform center = parent.GetComponent<BlackBeardMachineState>().CenterOfArena;

        // Gerar todos os ângulos igualmente espaçados
        List<float> angles = new();
        float angleStep = 360f / amountOfBullets;
        float startAngle = Random.Range(0f, 360f);

        for (int i = 0; i < amountOfBullets; i++) {
            angles.Add(startAngle + i * angleStep);
        }

        // Embaralhar a lista de ângulos
        for (int i = 0; i < angles.Count; i++) {
            int j = Random.Range(i, angles.Count);
            (angles[i], angles[j]) = (angles[j], angles[i]);
        }

        // Instanciar balas em ordem aleatória
        foreach (float angle in angles) {
            float rad = angle * Mathf.Deg2Rad;

            Vector3 position = center.position + new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;
            position.y += _info.BulletHeight;

            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent, position);

            yield return new WaitForSeconds(_info.TimeBetweenEachBullet);
        }

        End();
    }



}
