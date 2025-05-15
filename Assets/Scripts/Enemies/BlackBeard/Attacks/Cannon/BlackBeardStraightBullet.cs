using System.Collections;
using UnityEngine;

public class BlackBeardStraightBullet : EnemyAttackPrefab
{
    BlackBeardCannon _info;
    Vector3 pos;
    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardCannon;

        pos = position;

        DefinePosition();

    }

    private void DefinePosition() {
        Transform canon = parent.GetComponent<BlackBeardMachineState>().CannonsPosition[0];

        transform.localScale = Vector3.one * _info.StraightBulletSize;

        transform.SetPositionAndRotation(pos, canon.rotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        float duration = _info.StraightBulletRange / _info.StraightBulletSpeed;

        float timer = 0;

        Vector3 direction = -transform.forward;

        while (timer < duration) {
            timer += Time.deltaTime;
            transform.position += _info.StraightBulletSpeed * Time.deltaTime * direction;
            yield return null;
        }

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.DealDamage(_info.StraightBulletDamage, true, true);

        if (!other.TryGetComponent<MovementManager>(out MovementManager movement)) return;

        movement.StunWithTime(_info.StraightBulletStunTime);

        End();
    }
}
