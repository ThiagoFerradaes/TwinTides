using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrimsonTideExplosion : SkillObjectPrefab
{
    CrimsonTide _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;

    List<HealthManager> events = new();
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as CrimsonTide;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        DefineSizeAndPosition();
    }

    private void DefineSizeAndPosition() {
        transform.localScale = _info.ExplosionRadius * Vector3.one;

        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.ExplosionDuration);

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (_level == 4 && !events.Contains(health)) {
            health.OnDeath += Health_OnDeath;

            events.Add(health);
        }

        if (health.ReturnCurrentHealth() > health.ReturnMaxHealth() * _info.PercentToExecute / 100 || _level < 4) {
            float damage = _maevis.GetComponent<DamageManager>().ReturnTotalAttack(_info.ExplosionDamage);
            health.ApplyDamageOnServerRPC(damage, true, true);
        }
        else {
            health.ApplyDamageOnServerRPC(9999, false, false);
        }
    }

    private void Health_OnDeath() {
        _maevis.GetComponent<PlayerSkillManager>().ResetCooldown(_context.SkillIdInUI);
    }

    void End() {
        foreach (var enemies in events) {
            enemies.OnDeath -= Health_OnDeath;
        }
        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
