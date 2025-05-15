using System.Collections;
using UnityEngine;

public class ZombieTreeTwoPunch : EnemyAttackPrefab
{
    ZombieTreeTwo _info;

    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieTreeTwo;

        SetPosition();
    }

    void SetPosition() {

        Vector3 pos = parent.transform.position + parent.transform.forward * _info.punchPlacement;

        transform.SetPositionAndRotation(pos, parent.transform.rotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());

    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.punchDuration);

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.DealDamage(_info.punchDamage, true, true);
    }
}
