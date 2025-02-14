using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EchoBlastStunExplosion : SkillObjectPrefab
{
    EchoBlast _info;
    int _level;
    SkillContext _context;

    public static event EventHandler<ExplodedObject> OnExploded;

    public class ExplodedObject : EventArgs {
        public GameObject target;

        public ExplodedObject(GameObject target) {
            this.target = target;
        }
    }
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as EchoBlast;
        _level = skillLevel;
        _context = context;

        DefineSizeAndPosition();
    }

    void DefineSizeAndPosition() {
        if (_level < 2) {
            transform.localScale = _info.ExplosionRadius * Vector3.one;
        }
        else {
            transform.localScale = Vector3.one * _info.ExplosionRadiusLevel2;
        }

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

        if (!other.CompareTag("Enemey")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.ApplyDamageOnServerRPC(_info.ExplosionDamage, true, true);

        if (_level > 1) {
            // StunEnemies
        }

        if (_level > 2) {
            for(int i = 0; i < _info.ExplosionAmountLevel3; i++) {
                int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
                PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 2);
            }
        }

        if (_level > 3) {
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 3);
            OnExploded?.Invoke(this, new ExplodedObject(other.gameObject));
        }
    }
}
