using System;
using System.Collections;
using UnityEngine;

public class HullbreakerObject : SkillObjectPrefab {
    Hullbreaker _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Hullbreaker;
        _level = skillLevel;
        _context = context;

        SetParentAndPosition();
    }

    private void SetParentAndPosition() {
        if (_maevis == null) {
            _maevis = GameObject.FindGameObjectWithTag("Maevis");
        }

        transform.SetParent(_maevis.transform);

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        gameObject.SetActive(true);

        StartCoroutine(ShieldDuration());

        if (_level > 3) StartCoroutine(Earthquake());
    }

    IEnumerator ShieldDuration() {
        _maevis.TryGetComponent<HealthManager>(out HealthManager health);

        float duration;
        float shieldAmount;
        if (_level < 4) {
            duration = _info.ShieldDuration;
            if (_level < 2) shieldAmount = _info.ShieldAmount;
            else shieldAmount = _info.ShieldAmountLevel2;
        }
        else {
            duration = _info.ShieldDurationLevel4;
            shieldAmount = _info.ShieldAmountLevel4;
        }


        if (IsServer) {
            health.ApplyShieldServerRpc(shieldAmount, duration, true);
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            if (!health.isShielded.Value) break;
            yield return null;
        }

        Explode();

        ReturnObject();
    }

    IEnumerator Earthquake() {
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        while (true) {
            yield return new WaitForSeconds(_info.EarthquakeInterval);
            SkillContext newContext = new SkillContext(transform.position, transform.rotation, _context.SkillIdInUI);
            PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, newContext, _level, 2);
        }
    }

    void Explode() {
        if (_level < 3) return;
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);
        PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, newContext, _level, 1);
    }
}
