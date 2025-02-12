using System.Collections;
using UnityEngine;

public class SecondPhantomAuraObject : SkillObjectPrefab {
    PhantomAura _info;
    int _level;
    GameObject _maevis;
    SkillContext _context;

    bool _canDamage;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as PhantomAura;
        _level = skillLevel;

        SkillStart();
    }

    void SkillStart() {

        DefineSizeAndParent();

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

        if (_maevis != null) {
            transform.SetParent(_maevis.transform);
        }
        else {
            _maevis = GameObject.FindGameObjectWithTag("Maevis");
            transform.SetParent(_maevis.transform);
        }
        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);
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
        if (!_maevis.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float percentOfHealing = _info.HealingPercent / 100 * damage;

        health.HealServerRpc(percentOfHealing);
    }

    IEnumerator DamageTimer() {
        _canDamage = false;
        yield return new WaitForSeconds(_info.DamageInterval);
        _canDamage = true;
        yield return null;

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
