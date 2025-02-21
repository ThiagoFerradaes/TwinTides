using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;

public class DreadfallManager : SkillObjectPrefab {
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

        SetParentAndPosition();
    }

    private void SetParentAndPosition() {
        transform.parent = _maevis.transform;

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        gameObject.SetActive(true);

        StartCoroutine(JumpCoroutine());
    }

    IEnumerator JumpCoroutine() {

        Transform aim = _maevis.GetComponent<PlayerController>().aimObject;
        Vector3 targetPosition;

        if (aim != null && aim.gameObject.activeInHierarchy) {
            if (Vector3.Distance(_maevis.transform.position, aim.position) < _info.JumpMaxRange) targetPosition = aim.position;
            else targetPosition = _maevis.transform.position + _maevis.transform.forward * _info.JumpMaxRange;
        }
        else targetPosition = _maevis.transform.position + _maevis.transform.forward * _info.JumpMaxRange;


        _maevis.transform.DOJump(targetPosition, _info.JumpSpeed, 1, _info.JumpDuration);

        yield return new WaitForSeconds(_info.JumpDuration);

        RecieveShield();

        Explode();

        End();
    }

    private void RecieveShield() {
        if (_level < 2) return;

        if (!_maevis.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.ApplyShieldServerRpc(_info.AmountOfShiled, _info.ShieldDuration, true);
    }

    private void Explode() {
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);
        PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, newContext, _level, 1);
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
    void End() {
        _maevis.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, _info);
        ReturnObject();
    }
}
