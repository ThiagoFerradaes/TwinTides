using System;
using System.Collections;
using UnityEngine;

public class AreaWardStoneObject : SkillObjectPrefab {
    WardStone _info;
    int _level;
    SkillContext _context;

    bool _canHeal;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as WardStone;
        _level = skillLevel;
        _context = context;

        DefineSizeAndPosition();

    }

    private void DefineSizeAndPosition() {
        transform.localScale = _info.ExplosionRadiusLevel3;

        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(AreaDuration());
        StartCoroutine(HealingTimer());
    }

    IEnumerator AreaDuration() {
        if (_level < 4) {
            yield return new WaitForSeconds(_info.AreaDuration);
        }
        else {
            yield return new WaitForSeconds(_info.AreaDurationLevel4);
        }

        ReturnObject();
    }

    IEnumerator HealingTimer() {
        _canHeal = false;
        yield return new WaitForSeconds(_info.HealingInterval);
        _canHeal = true;
        yield return null;
        StartCoroutine(HealingTimer());
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Mel") || other.CompareTag("Maevis")) {
            if (!IsServer) return;

            if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

            if (!_canHeal) return;

            if (health.ReturnCurrentHealth() < health.maxHealth.Value) {
                Debug.Log("Heal");
                health.HealServerRpc(_info.AmountOfHealing);
            }
            else {
                Debug.Log("Shield");
                health.ApplyShieldServerRpc
                    (_info.AmountOfHealing * _info.PercentOfShieldFromExtraHealing / 100, _info.ExtraShieldDuration, true);
            }

        }
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
