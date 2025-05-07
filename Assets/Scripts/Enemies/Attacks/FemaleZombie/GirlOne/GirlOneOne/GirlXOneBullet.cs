using System.Collections;
using UnityEngine;

public class GirlXOneBullet : EnemyAttackPrefab
{
    GirlXOne _info;
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as GirlXOne;

        DefinePosition();

    }

    private void DefinePosition() {

        Vector3 pos = parent.transform.position + parent.transform.forward * _info.placementOfBullet;

        transform.SetPositionAndRotation(pos, parent.transform.rotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        float duration = _info.bulletRange / _info.bulletSpeed;

        float timer = 0;

        Vector3 direction = transform.forward;

        while (timer < duration) {
            timer += Time.deltaTime;
            transform.position += direction * _info.bulletSpeed * Time.deltaTime;
            yield return null;
        }

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.DealDamage(_info.damageOfBullet, true, true);

        End();
    }
}
