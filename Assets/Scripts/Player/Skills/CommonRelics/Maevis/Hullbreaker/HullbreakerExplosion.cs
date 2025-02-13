using System;
using System.Collections;
using UnityEngine;

public class HullbreakerExplosion : SkillObjectPrefab {

    Hullbreaker _info;
    SkillContext _context;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Hullbreaker;
        _context = context;

        DefinePosition();
    }

    private void DefinePosition() {
        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(ExplosionDuration());
    }

    IEnumerator ExplosionDuration() {
        yield return new WaitForSeconds(_info.ExplosionDuration);

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.ApplyDamageOnServerRPC(_info.ExplosionDamage, true, true);
    }
}
