using FMODUnity;
using System.Collections;
using UnityEngine;

public class GirlXOneStrongerBullet : EnemyAttackPrefab {
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

        if (!_info.StrongerShootSound.IsNull) RuntimeManager.PlayOneShot(_info.StrongerShootSound, transform.position);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        float duration = _info.StrongerBulletRange / _info.StrongerBulletSpeed;

        float timer = 0;

        Vector3 direction = transform.forward;

        while (timer < duration) {
            timer += Time.deltaTime;
            transform.position += _info.StrongerBulletSpeed * Time.deltaTime * direction;
            yield return null;
        }

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (_info is GirlTreeOne girl) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(girl, 4, parent, transform.position);
        }
        else {
            health.DealDamage(_info.StrongerBulletDamage, true, true);
        }

        End();
    }
}
