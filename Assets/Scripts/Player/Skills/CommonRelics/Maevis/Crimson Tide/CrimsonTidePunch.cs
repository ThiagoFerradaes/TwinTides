using System.Collections;
using UnityEngine;

public class CrimsonTidePunch : SkillObjectPrefab
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

        DefinePosition();
    }

    private void DefinePosition() {
        Vector3 direction = _context.PlayerRotation * Vector3.forward;
        Vector3 position = _context.PlayerPosition + (direction * _info.PunchAreaOffSett);
        transform.SetPositionAndRotation(position, _context.PlayerRotation);
        gameObject.SetActive(true);

        StartCoroutine(PunchDuration());
    }

    IEnumerator PunchDuration() {
        yield return new WaitForSeconds(_info.PunchAreaDuration);

        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.ApplyDamageOnServerRPC(_info.PunchDamage, true, true);
    }
}
