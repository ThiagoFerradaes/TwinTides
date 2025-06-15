using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBeardRainBulletBomb : BlackBeardAttackPrefab
{
    BlackBeardBulletRainSO _info;
    bool collided;
    HealthManager _health;
    bool isStronger;

    EventInstance sound;
    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardBulletRainSO;

        if (_health == null) _health = parent.GetComponent<HealthManager>();

        isStronger = _health.ReturnCurrentHealth() < (_health.ReturnMaxHealth() / 2);

        SetPosition(position);
    }

    void SetPosition(Vector3 position) {
        transform.position = position;

        transform.localScale = Vector3.one * _info.BulletSize;

        gameObject.SetActive(true);

        if (!_info.BulletFallingSound.IsNull) {
            sound = RuntimeManager.CreateInstance(_info.BulletFallingSound);
            RuntimeManager.AttachInstanceToGameObject(sound, this.gameObject);
            sound.start();
        }

        StartCoroutine(FallRoutine());
    }

    IEnumerator FallRoutine() {
        while (!collided) {
            transform.position += _info.BulletFallSpeed * Time.deltaTime * -transform.up;
            yield return null;
        }

        collided = false;

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent, transform.position, 0);

        if (isStronger) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 4, parent, transform.position, _info.AmountOfSecondaryBullets);
        }

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
    }
}
