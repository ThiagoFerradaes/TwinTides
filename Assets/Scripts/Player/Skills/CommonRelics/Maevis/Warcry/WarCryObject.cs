using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class WarCryObject : SkillObjectPrefab {
    Warcry _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    DamageManager _dManager;
    MovementManager _mManager;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Warcry;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            _dManager = _maevis.GetComponent<DamageManager>();
            _mManager = _maevis.GetComponent<MovementManager>();
        }

        DefineParentAndPosition();
    }

    private void DefineParentAndPosition() {

        transform.SetParent(_maevis.transform);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));


        gameObject.SetActive(true);

        Explode();

        StartCoroutine(Duration());
    }

    private void Explode() {
        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Maevis) return;

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        SkillContext newContex = new(transform.position, transform.rotation, _context.SkillIdInUI);
        PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, newContex, _level, 1);
    }

    IEnumerator Duration() {
        ApplyBuffs();

        float duration = _level < 4 ? _info.Duration : _info.DurationLevel4;

        yield return new WaitForSeconds(duration);

        ReturnObject();
    }

    void ApplyBuffs() {

        if (_level > 3) {
            LocalWhiteBoard.Instance.PlayerAttackSkill = _info.EnhancedMaevisAttack;
            _dManager.IncreaseAttackSpeed(_info.PercentAttackSpeedLevel4);
        }
        else {
            float percent = _level < 2 ? _info.PercentAttackSpeed : _info.PercentAttackSpeedLevel2;
            _dManager.IncreaseAttackSpeed(percent);
        }

        if (_level > 2) {
            _mManager.IncreaseMoveSpeed(_info.PercentMoveSpeedGain);
        }
    }
    void RemoveBuffs() {

        if (_level > 3) {
            LocalWhiteBoard.Instance.PlayerAttackSkill = _info.NormalMaevisAttack;
            _dManager.DecreaseAttackSpeed(_info.PercentAttackSpeedLevel4);
        }
        else {
            float percent = _level < 2 ? _info.PercentAttackSpeed : _info.PercentAttackSpeedLevel2;
            _dManager.DecreaseAttackSpeed(percent);
        }

        if (_level > 2) {
            _mManager.DecreaseMoveSpeed(_info.PercentMoveSpeedGain);
        }

    }

    public override void ReturnObject() {
        RemoveBuffs();
        base.ReturnObject();
    }
}
