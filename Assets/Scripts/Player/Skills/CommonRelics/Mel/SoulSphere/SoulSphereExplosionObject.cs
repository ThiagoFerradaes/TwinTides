using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SoulSphereExplosionObject : SkillObjectPrefab {
    SoulSphere _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as SoulSphere;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
        }

        DefineSize();

        StartCoroutine(Explode());
    }
    void DefineSize() {
        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        transform.localScale = _level < 4 ? Vector3.one * _info.ExplosionRadius : Vector3.one * _info.ExplosionRadiusLevel4;

        gameObject.SetActive(true);
    }
    IEnumerator Explode() {

        yield return new WaitForSeconds(_info.ExplosionDuration);

        if (_level >= 3) {
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 2);
        }

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (other.CompareTag("Maevis") || other.CompareTag("Mel")) {
            if (_level < 4) health.AddBuffToList(_info.invulnerabilityBuff);
        }

        if (!other.CompareTag("Enemy")) return;

        float damage = _mel.GetComponent<DamageManager>().ReturnTotalAttack(_info.ExplosionDamage);

        health.DealDamage(damage, true, true);
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
