using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class CrimsonTidePath : SkillObjectPrefab
{
    CrimsonTide _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    bool _canDamage;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as CrimsonTide;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        SetPosition();
    }
    void SetPosition() {
        transform.localScale = _info.PathSize;

        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());

        StartCoroutine(DamageCooldown());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.PathDuration);

        ReturnObject();
    }

    IEnumerator DamageCooldown() {
        while (true) {
            _canDamage = false;

            yield return new WaitForSeconds(_info.PathDamageInterval);

            _canDamage = true;

            yield return null;
        }
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }

    private void OnTriggerStay(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (!_canDamage) return;

        health.ApplyDamageOnServerRPC(_info.PathDamagePerTick, true, true);

    }
}
