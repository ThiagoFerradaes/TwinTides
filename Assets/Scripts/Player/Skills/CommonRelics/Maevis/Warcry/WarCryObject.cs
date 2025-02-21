using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WarCryObject : SkillObjectPrefab
{
    Warcry _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Warcry;
        _level = skillLevel;
        _context = context;

        DefineParentAndPosition();
    }

    private void DefineParentAndPosition() {
        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        transform.SetParent(_maevis.transform);

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        gameObject.SetActive(true);

        Explode();

        StartCoroutine(Duration());
    }

    private void Explode() {
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        SkillContext newContex = new(transform.position, transform.rotation, _context.SkillIdInUI);
        PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, newContex, _level, 1);
    }

    IEnumerator Duration() {
        ApplyBuffs();
        float duration;
        if (_level < 4) duration = _info.Duration;
        else duration = _info.DurationLevel4;

        yield return new WaitForSeconds(duration);

        End();
    }

    void ApplyBuffs() {
        if (_level > 3) {
            LocalWhiteBoard.Instance.PlayerAttackSkill = _info.EnhancedMaevisAttack;
        }
        if (_level > 2) {
            Debug.Log("Buff de velocidade de movimento aplicado a Maevis");
        }
        if (_level > 1) {
            Debug.Log("Buff de velocidade de ataque aumentada aplicado a Maevis");
        }
        else {
            Debug.Log("Buff de velocidade de ataque aplicado a Maevis");
        }
    }
    void RemoveBuffs() {
        if (_level > 3) {
            LocalWhiteBoard.Instance.PlayerAttackSkill = _info.NormalMaevisAttack;
        }
        if (_level > 2) {
            Debug.Log("Buff de velocidade de movimento removido a Maevis");
        }
        if (_level > 1) {
            Debug.Log("Buff de velocidade de ataque removido aplicado a Maevis");
        }
        else {
            Debug.Log("Buff de velocidade de ataque removido a Maevis");
        }
    }

    private void End() {
        RemoveBuffs();
        ReturnObject();
    }
}
