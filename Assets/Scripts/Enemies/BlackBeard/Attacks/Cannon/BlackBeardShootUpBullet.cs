using System.Collections;
using UnityEngine;

public class BlackBeardShootUpBullet : EnemyAttackPrefab
{
    BlackBeardCannon _info;
    bool collided;

    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardCannon;

        SetPosition(position);
    }

    void SetPosition(Vector3 position) {
        transform.position = position;

        transform.localScale = Vector3.one * _info.ShootUpBulletSize;

        gameObject.SetActive(true);

        StartCoroutine(FallRoutine());
    }

    IEnumerator FallRoutine() {
        while (!collided) {
            transform.position += _info.ShootUpBulletSpeedToFall * Time.deltaTime * -transform.up;
            yield return null;
        }

        collided = false;

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Floor")) {

            if (collided) return;

            collided = true;
        }
        else if (other.CompareTag("Mel") || other.CompareTag("Maevis")) {
           if(other.TryGetComponent<HealthManager>(out HealthManager health)) {
                health.DealDamage(_info.ShootUpBulletDamage, true, true);
            }
        }
    }
}
