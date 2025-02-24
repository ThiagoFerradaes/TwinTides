using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class DreadfallExplosion : SkillObjectPrefab {
    Dreadfall _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Dreadfall;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        SetPosition();
    }

    void SetPosition() {
        transform.localScale = _level < 3?  Vector3.one * _info.ExplosionRadius : Vector3.one * _info.ExplosionRadiusLevel3;

        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.ExplosionDuration);

        if (_level == 4 && IsServer) {
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 2);
        }

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        DamageManager dManager = _maevis.GetComponent<DamageManager>();

        float damage = _level < 3 ? dManager.ReturnTotalAttack(_info.ExplosionDamage) : dManager.ReturnTotalAttack(_info.ExplosionDamageLevel3);

        health.ApplyDamageOnServerRPC(damage, true, true);
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
