using FMODUnity;
using System.Collections;
using UnityEngine;

public class ZombieOneTwoAttack : EnemyAttackPrefab
{
    ZombieOneTwo _info;

    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieOneTwo;

        SetPosition();
    }

    private void SetPosition() {
        transform.localScale = Vector3.one * _info.normalAttackSize;

        transform.position = parent.transform.position;

        gameObject.SetActive(true);

        if (!_info.NormalAttackSound.IsNull) RuntimeManager.PlayOneShot(_info.NormalAttackSound, transform.position);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.normalAttackDuration);

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.DealDamage(_info.normalAttackDamage, true, true);
    }
}
