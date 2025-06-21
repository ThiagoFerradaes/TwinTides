using System;
using System.Collections;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using UnityEngine;

public class HullbreakerObject : SkillObjectPrefab {
    Hullbreaker _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    Animator anim;
    PlayerController _pController;
    PlayerSkillManager _sManager;

    Coroutine attackRoutine;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Hullbreaker;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            anim = _maevis.GetComponentInChildren<Animator>();
            _pController = _maevis.GetComponent<PlayerController>();
            _sManager = _maevis.GetComponent<PlayerSkillManager>();
        }

        SetParentAndPosition();
    }

    private void SetParentAndPosition() {

        transform.SetParent(_maevis.transform);

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        gameObject.SetActive(true);

        attackRoutine ??= StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine() {
        _pController.BlockMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(true);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);
        anim.SetTrigger("HullBreaker");

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        while (anim.IsInTransition(0)) yield return null;

        while (stateInfo.IsName(_info.AnimationName) == false) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        // Espera a animação terminar
        while (stateInfo.normalizedTime < _info.AnimationPercentToAttack) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        StartCoroutine(ShieldDuration());

        StartCoroutine(Earthquake());

        while (stateInfo.normalizedTime < 1f && stateInfo.IsName(_info.AnimationName)) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        _pController.AllowMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
        attackRoutine = null;
    }
    IEnumerator ShieldDuration() {
        _maevis.TryGetComponent<HealthManager>(out HealthManager health);


        health.ApplyShield(_info.ShieldAmount, _info.ShieldDuration, true);


        float elapsedTime = 0f;

        while (elapsedTime < _info.ShieldDuration) {
            elapsedTime += Time.deltaTime;
            if (!health.ReturnShieldStatus()) break;
            yield return null;
        }

        Explode();

        End();
    }

    IEnumerator Earthquake() {
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        while (true && LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
            yield return new WaitForSeconds(_info.EarthquakeInterval);
            SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, newContext, _level, 2);
        }
    }

    void Explode() {
        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Maevis) return;

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);
        PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, newContext, _level, 1);
    }
    void End() {

        if (_info.Character == LocalWhiteBoard.Instance.PlayerCharacter) {
            _maevis.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, _info);
        }

        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
