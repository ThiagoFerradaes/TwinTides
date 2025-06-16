using System.Collections.Generic;
using UnityEngine;

public class BlackHoleManager : SkillObjectPrefab
{
    BlackHole _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    Animator anim;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as BlackHole;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
            anim = _mel.GetComponentInChildren<Animator>();
        }

        if (_info.animationName != null) anim.SetTrigger(_info.animationName);

        gameObject.SetActive(true);

        InstantiateBlackHole();
    }

    void InstantiateBlackHole() {

        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Mel) return;

        Vector3 skillPos;

        skillPos.y = GetFloorHeight(_context.Pos);

        PlayerController controller = _mel.GetComponent<PlayerController>();
        Vector3 direction = _context.PlayerRotation * Vector3.forward;
        Vector3 position = _context.Pos + (direction * _info.MaxRange);

        if (LocalWhiteBoard.Instance.IsAiming && Vector3.Distance(controller.mousePos, _mel.transform.position) < _info.MaxRange) {

            skillPos.x = controller.mousePos.x;
            skillPos.z = controller.mousePos.z;
        }
        else {
            skillPos.x = position.x;
            skillPos.z = position.z;
        }

        SkillContext newContext = new(skillPos, transform.rotation, _context.SkillIdInUI);
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, newContext, _level, 1);

        End();
    }

    float GetFloorHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) return hit.point.y + 0.1f;
        return position.y;
    }

    void End() {
        ReturnObject();
    }

}
