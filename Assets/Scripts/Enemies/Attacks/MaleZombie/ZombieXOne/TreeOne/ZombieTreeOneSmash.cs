using FMODUnity;
using System.Collections;
using UnityEngine;

public class ZombieTreeOneSmash : EnemyAttackPrefab
{
    ZombieTreeOne _info;
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieTreeOne;

        DefinePosition();

    }

    private void DefinePosition() {

        Vector3 pos = parent.transform.position + parent.transform.forward * _info.smashPlacement;

        transform.SetPositionAndRotation(pos, parent.transform.rotation);

        gameObject.SetActive(true);

        if (!_info.SmashSound.IsNull) RuntimeManager.PlayOneShot(_info.SmashSound, transform.position);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.smashDuration);

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (!other.TryGetComponent<MovementManager>(out MovementManager movement)) return;

        movement.StunWithTime(_info.smashStunTime);

        health.DealDamage(_info.smashDamage, true, true);
    }
}
