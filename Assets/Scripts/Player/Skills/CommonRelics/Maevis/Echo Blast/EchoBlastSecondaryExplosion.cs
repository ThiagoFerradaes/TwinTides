using System;
using System.Collections;
using UnityEngine;
using static EchoBlastStunExplosion;

public class EchoBlastSecondaryExplosion : SkillObjectPrefab {
    EchoBlast _info;
    int _level;
    SkillContext _context;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as EchoBlast;
        _level = skillLevel;
        _context = context;

        DefineSizeAndPosition();
    }

    void DefineSizeAndPosition() {

        transform.localScale = Vector3.one * _info.ExplosionRadiusLevel2;

        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(ExplosionDuration());
    }

    IEnumerator ExplosionDuration() {
        yield return new WaitForSeconds(_info.ExplosionDuration);

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Enemey")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (IsServer) {
            health.ApplyDamageOnServerRPC(_info.ExplosionDamage, true, true);
        }

        health.AddDebuffToList(_info.BleedDebuff);
    }
}
