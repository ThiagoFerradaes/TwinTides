using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondPhantomAuraObject : SkillObjectPrefab {
    PhantomAura _info;
    int _level;
    GameObject _maevis;

    List<HealthManager> _listOfEnemies = new();

    EventInstance sound;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as PhantomAura;
        _level = skillLevel;

        if (_maevis == null) _maevis = PlayerSkillPooling.Instance.MaevisGameObject;

        DefineSizeAndParent();
    }

    void DefineSizeAndParent() {
        transform.localScale = _level < 4 ? _info.AuraSize : _info.AuraSizeLevel4;


        transform.SetParent(_maevis.transform);

        transform.SetLocalPositionAndRotation(Vector3.zero, _maevis.transform.rotation);


        gameObject.SetActive(true);

        if (_level < 4) {
            if (!_info.AuraSound.IsNull) {
                sound = RuntimeManager.CreateInstance(_info.AuraSound);
                RuntimeManager.AttachInstanceToGameObject(sound, this.gameObject);
                sound.start();
            }
        }
        else {
            if (!_info.StrongerAuraSound.IsNull) {
                sound = RuntimeManager.CreateInstance(_info.StrongerAuraSound);
                RuntimeManager.AttachInstanceToGameObject(sound, this.gameObject);
                sound.start();
            }
        }

        StartCoroutine(DamageTimer());

        StartCoroutine(Duration());
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager enemyHealth)) return;

        if (!_listOfEnemies.Contains(enemyHealth)) _listOfEnemies.Add(enemyHealth);
    }

    private void OnTriggerExit(Collider other) {

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager enemyHealth)) return;

        if (_listOfEnemies.Contains(enemyHealth)) _listOfEnemies.Remove(enemyHealth);
    }

    private void HealPlayer(float damage) {
        if (!_maevis.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float percentOfHealing = _info.HealingPercent / 100 * damage;

        health.Heal(percentOfHealing, true);
    }

    IEnumerator DamageTimer() {
        while (true) {
            yield return new WaitForSeconds(_info.DamageInterval);

            foreach (var enemyHealth in _listOfEnemies) {
                float damage = _level < 4 ? _maevis.GetComponent<DamageManager>().ReturnTotalAttack(_info.Damage) :
                    _maevis.GetComponent<DamageManager>().ReturnTotalAttack(_info.DamageLevel4);

                bool enemieDead = enemyHealth.ReturnDeathState();

                if (!enemieDead) {
                    enemyHealth.DealDamage(damage, true, true);

                    HealPlayer(damage);
                }
            }
        }
    }

    IEnumerator Duration() {
        float duration = _level < 4 ? _info.Duration : _info.DurationLevel4;

        yield return new WaitForSeconds(duration);

        End();
    }

    void End() {
        _listOfEnemies.Clear();
        transform.SetParent(null);

        if (sound.isValid()) {
            sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            sound.release();
        }

        ReturnObject();
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
