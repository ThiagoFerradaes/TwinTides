using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EchoBlastStunExplosion : SkillObjectPrefab
{
    EchoBlast _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    bool _canExplodeAgain = true;

    public static event EventHandler<ExplodedObject> OnExploded;
    public static event EventHandler<ExplosionPosition> OnSecondaryExplosion;

    public class ExplosionPosition : EventArgs {
        public SkillContext context;

        public ExplosionPosition(SkillContext context) {
            this.context = context;
        }
    }
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

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

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

        _canExplodeAgain = true;

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float damage = _maevis.GetComponent<DamageManager>().ReturnTotalAttack(_info.ExplosionDamage);

        health.ApplyDamageOnServerRPC(damage, true, true);

        if (_level > 1) {
            if (!other.TryGetComponent<MovementManager>(out MovementManager mManager)) return;

            mManager.StunWithTimeRpc(_info.StunTime);
        }

        if (_level > 2 && _canExplodeAgain) {
            _canExplodeAgain = false;
            OnSecondaryExplosion?.Invoke(this, new ExplosionPosition(_context));
        }

        if (_level > 3 && !health.ReturnDeathState()) {
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 4);
            OnExploded?.Invoke(this, new ExplodedObject(other.gameObject));
        }
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
