using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
using static EchoBlastStunExplosion;

public class EchoBlastSecondaryExplosion : SkillObjectPrefab {
    EchoBlast _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as EchoBlast;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        DefineSizeAndPosition();
    }

    void DefineSizeAndPosition() {

        transform.localScale = Vector3.one * _info.ExplosionRadiusLevel2;

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        if (!_info.SecondExplosionSound.IsNull) RuntimeManager.PlayOneShot(_info.SecondExplosionSound);

        StartCoroutine(ExplosionDuration());
    }

    IEnumerator ExplosionDuration() {
        yield return new WaitForSeconds(_info.ExplosionDuration);

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float damage = _maevis.GetComponent<DamageManager>().ReturnTotalAttack(_info.ExplosionDamage);
        health.DealDamage(damage, true, true);
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
