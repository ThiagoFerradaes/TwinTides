using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class BlackBeardWaveImpact : BlackBeardAttackPrefab {
    BlackBeardWaveAttackSO _info;
    Vector3 pos;
    HealthManager _health;
    bool isStronger;

    EventInstance sound;
    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        if (_info == null) _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardWaveAttackSO;

        if (_health == null) _health = parent.GetComponent<HealthManager>();

        pos = position;

        isStronger = _health.ReturnCurrentHealth() < (_health.ReturnMaxHealth() / 2);

        DefinePosition();

    }

    private void DefinePosition() {
        transform.localScale = Vector3.one * _info.WaveInitialRadius;

        transform.position = pos;

        gameObject.SetActive(true);

        if (!_info.WaveInstantiateSound.IsNull) RuntimeManager.PlayOneShot(_info.WaveInstantiateSound, parent.transform.position);

        if (!_info.WaveExpansionSound.IsNull) {
            sound = RuntimeManager.CreateInstance(_info.WaveExpansionSound);
            RuntimeManager.AttachInstanceToGameObject(sound, this.gameObject);
            sound.start();
        }

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        float speed = isStronger ? _info.WaveSpeedStronger : _info.WaveSpeed;
        float duration = _info.WaveMaxRadius / speed;

        float timer = 0;

        while (timer < duration) {
            timer += Time.deltaTime;
            float currentRadius = _info.WaveInitialRadius + timer * speed;
            transform.localScale = Vector3.one * currentRadius;
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

        health.DealDamage(_info.WaveDamage, true, true);

        if (isStronger) {
            _health.ApplyShield(_info.AmountOfShieldGainPerWaveHit, _info.ShieldDuration, true);
        }
    }
}
