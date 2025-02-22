using System;
using System.Collections;
using UnityEngine;

public class WarCryExplosion : SkillObjectPrefab {
    Warcry _info;
    int _level;
    SkillContext _context;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Warcry;
        _level = skillLevel;
        _context = context;

        DefineSizeAndPosition();
    }

    private void DefineSizeAndPosition() {
        if (_level < 3) transform.localScale = Vector3.one * _info.ExplosionRadius;
        else transform.localScale = Vector3.one * _info.ExplosionRadiusLevel3;

        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(ExplosionDuration());
    }

    IEnumerator ExplosionDuration() {
        yield return new WaitForSeconds(_info.ExplosionDuration);

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (other.CompareTag("Mel") && _level > 1) {

            if (_level > 3) {
                other.GetComponent<DamageManager>().IncreaseBaseAttackWithTimeRpc(_info.PercentAttackSpeedLevel4, _info.DurationLevel4);
            }
            else {
                other.GetComponent<DamageManager>().IncreaseBaseAttackWithTimeRpc(_info.PercentAttackSpeedLevel2, _info.Duration);
            }

            if (_level > 2) {
                other.GetComponent<MovementManager>().IncreaseMoveSpeedWithTimeRpc(_info.PercentMoveSpeedGain, _info.Duration);
            }
        }

        if (other.CompareTag("Enemy") && _level > 2) {
            other.GetComponent<MovementManager>().StunWithTimeRpc(_info.StunDuration);
        }
    }
}
