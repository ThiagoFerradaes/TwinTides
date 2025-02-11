using System;
using System.Collections;
using UnityEngine;

public class PhantomAuraObject : SkillObjectPrefab {
    PhantomAura _info;
    int _level;
    SkillContext _context;
    GameObject _mel;

    bool _canDamage;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as PhantomAura;
        _level = skillLevel;
        _context = context;
        SkillStart();
    }

    void SkillStart() {

        DefineSizeAndParent();

        SecondAura();

        gameObject.SetActive(true);

        StartCoroutine(DamageTimer());

        StartCoroutine(Duration());
    }

    void DefineSizeAndParent() {
        if (_level < 4) {
            transform.localScale = _info.AuraSize;
        }
        else {
            transform.localScale = _info.AuraSizeLevel4;
        }
        if (_mel != null) {
            transform.SetParent(_mel.transform);
        }
        else {
            _mel = GameObject.FindGameObjectWithTag("Mel");
            transform.SetParent(_mel.transform);
        }
        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);
    }

    void SecondAura() {
        if (_level >= 3) {
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 1);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (!IsServer) return;

        if (!_canDamage || other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager enemyHealth)) return;

        if (_level < 4) {
            enemyHealth.ApplyDamageOnServerRPC(_info.Damage, true, true);
            if (_level >= 2) {
                HealPlayer(_info.Damage);
            }
        }
        else {
            enemyHealth.ApplyDamageOnServerRPC(_info.DamageLevel4, true, true);

            HealPlayer(_info.DamageLevel4);
        }
    }

    private void HealPlayer(float damage) {
        if (!_mel.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float percentOfHealing = _info.HealingPercent / 100 * damage;

        health.HealServerRpc(percentOfHealing);
    }

    IEnumerator DamageTimer() {
        _canDamage = false;
        yield return new WaitForSeconds(_info.DamageInterval);
        _canDamage = true;

        StartCoroutine(DamageTimer());
    }

    IEnumerator Duration() {
        if (_level < 4) {
            yield return new WaitForSeconds(_info.Duration);
        }
        else {
            yield return new WaitForSeconds(_info.DurationLevel4);
        }
        transform.SetParent(null);
        ReturnObject();
    }
}
