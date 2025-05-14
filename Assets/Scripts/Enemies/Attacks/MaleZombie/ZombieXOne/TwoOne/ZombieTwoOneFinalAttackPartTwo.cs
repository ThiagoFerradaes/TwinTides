using System.Collections;
using UnityEngine;

public class ZombieTwoOneFinalAttackPartTwo : EnemyAttackPrefab
{
    ZombieTwoOne _info;
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieTwoOne;

        DefinePosition();

    }

    private void DefinePosition() {

        Vector3 pos = parent.transform.position + parent.transform.forward * _info.placementOfFinalAttackPartTwo;

        transform.SetPositionAndRotation(pos, parent.transform.rotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.durationOfFinalAttackPartTwo);

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (!other.TryGetComponent<MovementManager>(out MovementManager movement)) return;

        movement.StunWithTime(_info.stunTimeFinalAttackTwo);

        health.DealDamage(_info.damageOfFinalAttackPartTwo, true, true);
    }
}
