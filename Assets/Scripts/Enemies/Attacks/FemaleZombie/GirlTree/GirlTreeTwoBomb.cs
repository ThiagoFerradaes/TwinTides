using FMODUnity;
using System.Collections;
using UnityEngine;

public class GirlTreeTwoBomb : EnemyAttackPrefab
{
    GirlTreeTwo _info;
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as GirlTreeTwo;

        DefinePosition();

    }

    private void DefinePosition() {

        Vector3 pos = parent.transform.position + parent.transform.forward * _info.bombPlacement;

        transform.SetPositionAndRotation(pos, parent.transform.rotation);

        gameObject.SetActive(true);

        if (!_info.ShootSound.IsNull) RuntimeManager.PlayOneShot(_info.ShootSound, transform.position);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        float duration = _info.bombsRange / _info.bombsSpeed;

        float timer = 0;

        Vector3 direction = transform.forward;

        while (timer < duration) {
            timer += Time.deltaTime;
            transform.position += direction * _info.bombsSpeed * Time.deltaTime;
            yield return null;
        }

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent, transform.position);

        End();
    }
}
