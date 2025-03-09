using System.Collections;
using UnityEngine;

public class CrimsonTideObject : SkillObjectPrefab {

    CrimsonTide _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    PlayerSkillManager _skillManager;
    PlayerController _playerController;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as CrimsonTide;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            _skillManager = _maevis.GetComponent<PlayerSkillManager>();
            _playerController = _maevis.GetComponent<PlayerController>();
        }

        DefineParent();
    }

    private void DefineParent() {
        if (IsServer) {
            transform.SetParent(_maevis.transform);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));
        }

        gameObject.SetActive(true);

        _skillManager.BlockNormalAttackRpc(true);
        _skillManager.BlockSkillsRpc(true);

        if (_level < 2) {
            if (IsServer) {
                int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
                PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 1);
            }
            End();
        }
        else {
            StartCoroutine(Dash());

            if (_level == 4 && IsServer) StartCoroutine(SpawnPath());
        }
    }
    IEnumerator Dash() {
        float elapsedTime = 0f;

        _playerController.BlockMovement();

        if (IsServer) {
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
        }
        else {
            yield return new WaitForSeconds(_info.DashDuration);
        }

        End();
    }

    IEnumerator SpawnPath() {
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        Vector3 lastSpawnPosition = _maevis.transform.position;
        while (true) {
            if (Vector3.Distance(lastSpawnPosition, _maevis.transform.position) >= _info.PathSpawnInterval) {
                SkillContext newContext = new(_maevis.transform.position, transform.rotation, _context.SkillIdInUI);
                PlayerSkillPooling.Instance.InstantiateAndSpawnNoCheckRpc(skillId, newContext, _level, 4);
                lastSpawnPosition = _maevis.transform.position;
            }
            yield return null;
        }
    }

    void End() {
        _maevis.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
        _maevis.GetComponent<PlayerController>().AllowMovement();
        ReturnObject();
    }
}
