using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SoulSphereExplosionObject : SkillObjectPrefab {
    SoulSphere _info;
    int _level;
    SkillContext _context;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as SoulSphere;
        _level = skillLevel;
        _context = context;

        DefineSize();

        StartCoroutine(Explode());
    }
    void DefineSize() {
        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        if (_level < 4) {
            transform.localScale = Vector3.one * _info.ExplosionRadius;
        }
        else {
            transform.localScale = Vector3.one * _info.ExplosionRadius;
            transform.localScale *= _info.ExplosionRadiusMultiplier;
        }

        gameObject.SetActive(true);
    }
    IEnumerator Explode() {

        yield return new WaitForSeconds(_info.ExplosionDuration);

        if (_level >= 3) {
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 2);
        }

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Enemy") && !IsServer) return;

        if (other.TryGetComponent<HealthManager>(out HealthManager health)) {
            health.ApplyDamageOnServerRPC(_info.ExplosionDamage, true, true);
        }
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
