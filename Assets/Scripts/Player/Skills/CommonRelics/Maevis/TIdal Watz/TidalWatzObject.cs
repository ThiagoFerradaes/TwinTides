using System;
using System.Collections;
using UnityEngine;

public class TidalWatzObject : SkillObjectPrefab {

    TidalWatz _info;
    int _level;
    SkillContext _context;

    GameObject _maevis;
    [HideInInspector] public float acumulativeDamage;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as TidalWatz;
        _level = skillLevel;
        _context = context;

        DefineParent();
    }

    private void DefineParent() {
        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        transform.SetParent(_maevis.transform);

        transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(0,0,0));

        _maevis.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(true);
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);

        gameObject.SetActive(true);

        StartCoroutine(Cut());
    }

    IEnumerator Cut() {
        int amountOfCuts = _level switch {
            1 => _info.AmountOfCuts,
            2 => _info.AmountOfCutsLevel2,
            3 => _info.AmountOfCutsLevel3,
            _ => _info.AmountOfCutsLevel4
        };

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);


        for (int i = 0; i < amountOfCuts; i++) {
            PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 1);

            float startAngle = transform.localEulerAngles.y;
            float targetAngle = startAngle + 360;

            float elapsedTime = 0f;
            float duration;
            if (_level < 3) {
                duration = _info.CutDuration;
            }
            else {
                duration = _info.CutDurationLevel3;
            }

            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                float yRotation = Mathf.Lerp(startAngle, targetAngle, t);
                transform.localRotation = Quaternion.Euler(0, yRotation, 0);
                yield return null;
            }

            transform.localRotation = Quaternion.Euler(0,startAngle,0);

            if (_level < 3 && _level > 1) {
                yield return new WaitForSeconds(_info.CutInterval);
            }
            else {
                yield return new WaitForSeconds(_info.CutIntervalLevel3);
            }
        }

        if (_level == 4) {
            PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 2);
            yield return new WaitForSeconds(_info.ImpactDuration);
        }

        End();
    }

    void End() {
        acumulativeDamage = 0;
        _maevis.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, _info);
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
        _maevis.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        ReturnObject();
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
