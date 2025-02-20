using System;
using System.Collections;
using UnityEngine;

public class EchoBlastExplodingDebuff : SkillObjectPrefab {
    EchoBlast _info;
    bool _canExplode, _canSetUpExplosion;
    int _level;
    SkillContext _context;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as EchoBlast;
        _level = skillLevel;
        _context = context;

        EchoBlastStunExplosion.OnExploded += EchoBlastStunExplosion_OnExploded;
    }

    private void DefineParent(GameObject parent) {
        transform.SetParent(parent.transform);
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        parent.TryGetComponent<HealthManager>(out HealthManager health);

        health.OnGeneralDamage += Health_OnGeneralDamage;

        StartCoroutine(DebuffDuration());
    }

    private void Health_OnGeneralDamage(object sender, EventArgs e) {
        CanExplode();
    }

    private void EchoBlastStunExplosion_OnExploded(object sender, EchoBlastStunExplosion.ExplodedObject e) {
        DefineParent(e.target);
    }

    IEnumerator DebuffDuration() {
        StartCoroutine(Explode());
        yield return new WaitForSeconds(_info.ExplodingDebuffDuration);

        End();
    }

    IEnumerator Explode() {
        while (true) {
            if (_canExplode) {
                _canExplode = false;
                _canSetUpExplosion = false;
                int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
                SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);
                PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, newContext, _level, 2);

                StartCoroutine(ExplosionCooldown());
            }
        }
    }

    IEnumerator ExplosionCooldown() {
        yield return new WaitForSeconds(_info.ExplodingDebuffExplosionCooldown);

        _canSetUpExplosion = true;
    }

    void CanExplode() {
        if (_canSetUpExplosion) _canExplode = true;
    }

    void End() {
        EchoBlastStunExplosion.OnExploded -= EchoBlastStunExplosion_OnExploded;
        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
