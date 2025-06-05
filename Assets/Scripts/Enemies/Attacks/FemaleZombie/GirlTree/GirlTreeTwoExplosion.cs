using FMODUnity;
using System.Collections;
using UnityEngine;

public class GirlTreeTwoExplosion : EnemyAttackPrefab
{
    GirlTreeTwo _info;
    Vector3 _position;
    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        _position = position;

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as GirlTreeTwo;

        DefinePosition();

    }

    private void DefinePosition() {

        Vector3 pos = _position;

        transform.localScale = Vector3.one * _info.explosionRadius;

        transform.SetPositionAndRotation(pos, parent.transform.rotation);

        gameObject.SetActive(true);

        if (!_info.ExplosionSound.IsNull) RuntimeManager.PlayOneShot(_info.ExplosionSound, transform.position);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.explosionDuration);

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Maevis") || other.CompareTag("Mel")) {

            if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

            health.DealDamage(_info.explosionDamage, true, true);
            health.AddDebuffToList(_info.burningDebuff);
        }

        if (other.CompareTag("Oil")) {
           if (!other.TryGetComponent<ZombieTreeTwoOil>(out ZombieTreeTwoOil oil)) return;
            oil.Burn();
        }

        End();
    }
}
