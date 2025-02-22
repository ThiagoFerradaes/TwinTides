using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class CrimsonTidePath : SkillObjectPrefab
{
    CrimsonTide _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    bool _canDamage;

    private List<HealthManager> events = new();

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as CrimsonTide;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        SetPosition();
    }
    void SetPosition() {
        transform.localScale = _info.PathSize;

        _context.PlayerPosition.y = GetGroundHeight(_context.PlayerPosition);

        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());

        StartCoroutine(DamageCooldown());
    }

    float GetGroundHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) {
            return hit.point.y + 0.1f;
        }
        return position.y; 
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.PathDuration);

        End();
    }

    IEnumerator DamageCooldown() {
        while (true) {
            _canDamage = false;

            yield return new WaitForSeconds(_info.PathDamageInterval);

            _canDamage = true;

            yield return null;
        }
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }

    private void OnTriggerStay(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (!events.Contains(health)) {
            health.OnDeath += Health_OnDeath;

            events.Add(health);
        }

        if (!_canDamage) return;

        if (health.ReturnCurrentHealth() > health.ReturnMaxHealth() * _info.PercentToExecute / 100) {
            float damage = _maevis.GetComponent<DamageManager>().ReturnTotalAttack(_info.PathDamagePerTick);
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
        foreach(var enemy in events) {
            enemy.OnDeath -= Health_OnDeath;
        }
        events.Clear();

        ReturnObject();
    }
}
