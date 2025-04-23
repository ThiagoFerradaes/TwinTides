using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class EchoBlastStunExplosion : SkillObjectPrefab {
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
        transform.localScale = _level < 2 ? Vector3.one * _info.ExplosionRadius : Vector3.one * _info.ExplosionRadiusLevel2;

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(ExplosionDuration());
    }

    IEnumerator ExplosionDuration() {
        yield return new WaitForSeconds(_info.ExplosionDuration);

        _canExplodeAgain = true;

        End();
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float damage = _maevis.GetComponent<DamageManager>().ReturnTotalAttack(_info.ExplosionDamage);

        health.DealDamage(damage, true, true);

        if (_level > 1) {
            if (!other.TryGetComponent<MovementManager>(out MovementManager mManager)) return;

            mManager.StunWithTime(_info.StunTime);
        }

        if (_level > 2 && _canExplodeAgain) {
            _canExplodeAgain = false;
            OnSecondaryExplosion?.Invoke(this, new ExplosionPosition(_context));
        }

        if (_level > 3 && !health.ReturnDeathState()) {
            if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
                int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
                PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 4);
            }
            OnExploded?.Invoke(this, new ExplodedObject(health.gameObject));
        }
    }

    void End() {

        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
