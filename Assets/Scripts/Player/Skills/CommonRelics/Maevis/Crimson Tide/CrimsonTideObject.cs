using System;
using System.Collections;
using TreeEditor;
using UnityEngine;

public class CrimsonTideObject : SkillObjectPrefab {

    CrimsonTide _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as CrimsonTide;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        DefineParent();
    }

    private void DefineParent() {
        transform.SetParent(_maevis.transform);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        gameObject.SetActive(true);

        if (_level < 2) {
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 1);
            End();
        }
        else {
            StartCoroutine(Dash());
            if (_level == 4) StartCoroutine(SpawnPath());
        }
    }

    IEnumerator Dash() {
        float elapsedTime = 0f;

        _maevis.GetComponent<PlayerController>().BlockMovement();

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 2);

        while (elapsedTime < _info.DashDuration) {
            elapsedTime += Time.deltaTime;
            _maevis.transform.position += _info.DashSpeed * Time.deltaTime * _maevis.transform.forward;
            yield return null;
        }

        if (_level > 2) {
            SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);
            PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, newContext, _level, 3);
        }

        _maevis.GetComponent<PlayerController>().AllowMovement();

        End();
    }

    IEnumerator SpawnPath() {
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        while (true) {
            SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);
            PlayerSkillPooling.Instance.InstantiateAndSpawnNoCheckRpc(skillId, newContext, _level, 4);
            yield return new WaitForSeconds(_info.PathSpawnInterval);
        }
    }

    void End() {
        _maevis.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, _info);
        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
