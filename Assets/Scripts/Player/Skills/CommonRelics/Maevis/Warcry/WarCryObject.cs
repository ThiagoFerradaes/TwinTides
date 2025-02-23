using System;
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
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

        DefineParentAndPosition();
    }

    private void DefineParentAndPosition() {
        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        _dManager = _maevis.GetComponent<DamageManager>();
        _mManager = _maevis.GetComponent<MovementManager>();

        if (IsServer) {
            if (_maevis.TryGetComponent<NetworkObject>(out NetworkObject net)) {
                transform.SetParent(net.transform);
            }
        }

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
        if (!IsServer) return;

        if (_level > 3) {
            LocalWhiteBoard.Instance.PlayerAttackSkill = _info.EnhancedMaevisAttack;
            _dManager.IncreaseAttackSpeedRpc(_info.PercentAttackSpeedLevel4);
        }
        else {
            if (_level > 1) {
                _dManager.IncreaseAttackSpeedRpc(_info.PercentAttackSpeedLevel2);
            }
            else {
                _dManager.IncreaseAttackSpeedRpc(_info.PercentAttackSpeed);
            }
        }

        if (_level > 2) {
            _mManager.IncreaseMoveSpeedRpc(_info.PercentMoveSpeedGain);
        }
    }
    void RemoveBuffs() {
        if (!IsServer) return;

        if (_level > 3) {
            LocalWhiteBoard.Instance.PlayerAttackSkill = _info.NormalMaevisAttack;
            _dManager.DecreaseAttackSpeedRpc(_info.PercentAttackSpeedLevel4);
        }
        else {
            if (_level > 1) {
                _dManager.DecreaseAttackSpeedRpc(_info.PercentAttackSpeedLevel2);
            }
            else {
                _dManager.DecreaseAttackSpeedRpc(_info.PercentAttackSpeed);
            }
        }

        if (_level > 2) {
            _mManager.DecreaseMoveSpeedRpc(_info.PercentMoveSpeedGain);
        }

    }

    private void End() {
        RemoveBuffs();
        ReturnObject();
    }
}
