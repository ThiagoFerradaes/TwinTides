using System.Collections;
using UnityEngine;

public class ZombieTreeOnePunch : EnemyAttackPrefab
{
    ZombieTreeOne _info;
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieTreeOne;

        DefinePosition();

    }

    private void DefinePosition() {

        Vector3 pos = parent.transform.position + parent.transform.forward * _info.placementOfPunch;

        transform.SetPositionAndRotation(pos, parent.transform.rotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.durationOfPunch);

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.DealDamage(_info.damagePerPunch, true, true);
    }
}
