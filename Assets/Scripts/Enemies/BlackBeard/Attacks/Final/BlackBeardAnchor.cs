using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class BlackBeardAnchor : BlackBeardAttackPrefab {
    BlackBeardAnchorAttackSO _info;
    Vector3 pos;
    Vector3 _direction;
    HealthManager health;
    bool isStronger;

    EventInstance sound;
    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        if (_info == null) _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardAnchorAttackSO;

        if (health == null) health = parent.GetComponent<HealthManager>();

        pos = position;

        isStronger = health.ReturnCurrentHealth() < (health.ReturnMaxHealth() / 2);

        DefinePosition();

    }

    private void DefinePosition() {
        transform.localScale = _info.AnchorSize;

        _direction = (pos - parent.transform.position).normalized;

        transform.SetPositionAndRotation(pos, Quaternion.LookRotation(_direction));

        gameObject.SetActive(true);

        if (!_info.AnchorMovementDownSound.IsNull) {
            sound = RuntimeManager.CreateInstance(_info.AnchorMovementDownSound);
            RuntimeManager.AttachInstanceToGameObject(sound, this.gameObject);
            sound.start();
        }

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        float duration = _info.AnchorRange / _info.AnchorSpeed;

        float timer = 0;

        Vector3 fowardDirection = transform.forward;

        while (timer < duration) {
            timer += Time.deltaTime;
            transform.position += _info.AnchorSpeed * Time.deltaTime * fowardDirection;
            yield return null;
        }

        if (sound.isValid()) {
            sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            sound.release();
        }

        if (isStronger) {
            transform.SetParent(parent.transform);
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent, transform.position);

            if (_info.AnchorSpinningSound.IsNull) {
                sound = RuntimeManager.CreateInstance(_info.AnchorSpinningSound);
                RuntimeManager.AttachInstanceToGameObject(sound, parent);
                sound.start();
            }

            yield return new WaitForSeconds(_info.RotationDuration);

            if (sound.isValid()) {
                sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                sound.release();
            }

            transform.SetParent(null);
        }
        else {
            yield return new WaitForSeconds(_info.TimeBetweenAttacks);
        }

        Vector3 returnDirection = (parent.transform.position - transform.position).normalized;
        timer = 0;

        while (timer < duration) {
            timer += Time.deltaTime;
            transform.position += _info.AnchorSpeed * Time.deltaTime * returnDirection;
            yield return null;
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
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.DealDamage(_info.AnchorDamage, true, true);

        if (!other.TryGetComponent<MovementManager>(out MovementManager move)) return;

        move.StunWithTime(_info.AnchorStunTime);

        if (_info.AnchorHitSound.IsNull) RuntimeManager.PlayOneShot(_info.AnchorHitSound, transform.position);
    }
}
