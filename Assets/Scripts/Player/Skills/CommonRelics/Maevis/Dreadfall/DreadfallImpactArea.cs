using System;
using System.Collections;
using UnityEngine;

public class DreadfallImpactArea : SkillObjectPrefab
{

    Dreadfall _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;

    bool _canDamage = true;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Dreadfall;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        SetPosition();
    }

    private void SetPosition() {

        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());

        StartCoroutine(DamageCooldown());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.FieldDuration);

        ReturnObject();
    }

    IEnumerator DamageCooldown() {
        while (true) {
            yield return null;
            _canDamage = false;
            yield return new WaitForSeconds(_info.DamageCooldown);
            _canDamage = true;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (!_canDamage) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (IsServer) health.ApplyDamageOnServerRPC(_info.FieldDamagePerTick, true, true);

        health.AddDebuffToList(_info.BleedDebuff);
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
