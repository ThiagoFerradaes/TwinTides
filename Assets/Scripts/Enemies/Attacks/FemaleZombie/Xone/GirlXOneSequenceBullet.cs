using FMODUnity;
using System.Collections;
using UnityEngine;

public class GirlXOneSequenceBullet : EnemyAttackPrefab
{
    GirlTwoOne _info;
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as GirlTwoOne;

        DefinePosition();

    }

    private void DefinePosition() {

        Vector3 pos = parent.transform.position + parent.transform.forward * _info.placementOfBullet;

        transform.SetPositionAndRotation(pos, parent.transform.rotation);

        gameObject.SetActive(true);

        if (!_info.SequenceBulletShootSound.IsNull) RuntimeManager.PlayOneShot(_info.SequenceBulletShootSound, transform.position);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        float duration = _info.finalSequenceBulletsRange / _info.finalSequenceBulletsSpeed;

        float timer = 0;

        Vector3 direction = transform.forward;

        while (timer < duration) {
            timer += Time.deltaTime;
            transform.position += direction * _info.finalSequenceBulletsSpeed * Time.deltaTime;
            yield return null;
        }

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.DealDamage(_info.finalSequenceBulletsDamage, true, true);

        End();
    }
}
