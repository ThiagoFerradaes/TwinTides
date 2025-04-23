using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class FallenBannerManager : SkillObjectPrefab {
    FallenBanner _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as FallenBanner;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        gameObject.SetActive(true);

        InstantiateBanners();

        End();
    }

    void InstantiateBanners() {
        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Maevis) return;

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);

        if (_level < 2) {
            Vector3 skillPos;

            skillPos.y = GetFloorHeight(_context.Pos);

            Transform aim = _maevis.GetComponent<PlayerController>().aimObject;
            Vector3 direction = _context.PlayerRotation * Vector3.forward;
            Vector3 position = _context.Pos + (direction * _info.MaxRange);

            if (aim != null && aim.gameObject.activeInHierarchy && Vector3.Distance(_maevis.transform.position, aim.position) <= _info.MaxRange) {
                Debug.Log("Aim on");
                skillPos.x = aim.position.x;
                skillPos.z = aim.position.z;
            }
            else {
                skillPos.x = position.x;
                skillPos.z = position.z;
            }

            SkillContext newContext = new(skillPos, transform.rotation, _context.SkillIdInUI);
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, newContext, _level, 1);
        }

        else {
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 1);

            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 2);
        }
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
