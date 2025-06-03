using FMODUnity;
using System;
using System.Collections;
using UnityEngine;

public class HullbreakerExplosion : SkillObjectPrefab {

    Hullbreaker _info;
    SkillContext _context;
    GameObject _maevis;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Hullbreaker;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        DefinePosition();
    }

    private void DefinePosition() {
        transform.localScale = Vector3.one * _info.ExplosionRadius;

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        if (!_info.ExplosionSound.IsNull) RuntimeManager.PlayOneShot(_info.ExplosionSound, transform.position);

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
