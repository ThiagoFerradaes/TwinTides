using System.Collections.Generic;
using UnityEngine;

public class PhantomAuraManager : SkillObjectPrefab {
    PhantomAura _info;
    int _level;
    SkillContext _context;
    GameObject _mel;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as PhantomAura;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
        }

        gameObject.SetActive(true);

        InstantiateAuras();

        End();
    }

    void InstantiateAuras() {
        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Mel) return;

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);

        if (_level > 2) {
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 1);

            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 2);
        }
        else {
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 1);
        }
    }

    void End() {
        ReturnObject();
    }
}
