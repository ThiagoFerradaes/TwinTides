using System;
using System.Collections;
using UnityEngine;

public class CrimsonTideExplosion : SkillObjectPrefab
{
    CrimsonTide _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;

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

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.ApplyDamageOnServerRPC(_info.ExplosionDamage, true, true);
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
