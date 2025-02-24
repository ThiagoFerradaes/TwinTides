using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrimsonTideDashHitBox : SkillObjectPrefab {
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

        SetParent();
    }

    private void SetParent() {
        if (IsServer) {
            transform.SetParent(_maevis.transform);

            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

            gameObject.SetActive(true);

            StartCoroutine(Duration());
        }
        else {
            gameObject.SetActive(true);
        }
        

    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.DashDuration);

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
            float damage = _maevis.GetComponent<DamageManager>().ReturnTotalAttack(_info.DashDamage);
            health.ApplyDamageOnServerRPC(damage, true, true);
        }
        else {
            health.ApplyDamageOnServerRPC(9999, false, false);
        }

    }

    private void Health_OnDeath() {
        if (_info.Character == LocalWhiteBoard.Instance.PlayerCharacter)
            _maevis.GetComponent<PlayerSkillManager>().ResetCooldown(_context.SkillIdInUI);
    }

    void End() {
        foreach (var enemies in events) {
            enemies.OnDeath -= Health_OnDeath;
        }
        events.Clear();
        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
