using FMODUnity;
using System.Collections;
using UnityEngine;

public class GirlXOneExplosion : EnemyAttackPrefab
{
    GirlTreeOne _info;
    Vector3 _position;
    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        _position = position;

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as GirlTreeOne;

        DefinePosition();

    }

    private void DefinePosition() {

        Vector3 pos = _position;

        transform.localScale = Vector3.one * _info.ExplosionSize;

        transform.SetPositionAndRotation(pos, parent.transform.rotation);

        gameObject.SetActive(true);

        if (!_info.ExplosionSound.IsNull) RuntimeManager.PlayOneShot(_info.ExplosionSound, transform.position);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.ExplosionDuration);

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.DealDamage(_info.ExplosionDamage, true, true);

        End();
    }
}
