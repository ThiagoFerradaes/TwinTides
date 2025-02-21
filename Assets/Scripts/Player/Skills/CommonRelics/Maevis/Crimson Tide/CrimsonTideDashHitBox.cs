using System;
using System.Collections;
using UnityEngine;

public class CrimsonTideDashHitBox : SkillObjectPrefab
{
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

        SetParent();
    }

    private void SetParent() {
        transform.SetParent(_maevis.transform);

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.DashDuration);

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.ApplyDamageOnServerRPC(_info.DashDamage, true, true);
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
