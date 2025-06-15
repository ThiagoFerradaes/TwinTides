using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class BlackBeardShootUpBullet : BlackBeardAttackPrefab {
    BlackBeardCannon _info;
    bool collided;

    EventInstance sound;

    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardCannon;

        SetPosition(position);
    }

    void SetPosition(Vector3 position) {
        transform.position = position;

        transform.localScale = Vector3.one * _info.ShootUpBulletSize;

        gameObject.SetActive(true);

        if (!_info.ShootUpFallingSound.IsNull) {
            sound = RuntimeManager.CreateInstance(_info.ShootUpFallingSound);
            RuntimeManager.AttachInstanceToGameObject(sound, this.gameObject);
            sound.start();
        }

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

    public override void End() {

        if (sound.isValid()) {
            sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            sound.release();
        }

        base.End();
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
