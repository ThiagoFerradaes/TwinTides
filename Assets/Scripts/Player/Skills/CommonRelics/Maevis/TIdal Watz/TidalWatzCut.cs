using System;
using System.Collections;
using UnityEngine;

public class TidalWatzCut : SkillObjectPrefab
{
    TidalWatz _info;
    int _level;

    TidalWatzObject _father;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as TidalWatz;
        _level = skillLevel;

        SetSizeAndPosition();
    }

    private void SetSizeAndPosition() {
        if (_father == null) {
            _father = GameObject.FindAnyObjectByType<TidalWatzObject>();
        }

        if (_level == 1) {
            transform.localScale = _info.CutSize;
        }
        else {
            transform.localScale = _info.CutSizeLevel2;
        }

        transform.SetParent(_father.transform);

        Vector3 position;

        if (_level < 2) {
            position = _info.CutPosition;
        }
        else {
            position = _info.CutPositionLevel2;
        }

        transform.SetLocalPositionAndRotation(position, Quaternion.Euler(0,0,0));

        gameObject.SetActive(true);

        StartCoroutine(CutDuration());
    }

    IEnumerator CutDuration() {
        float duration;
        if (_level < 3) {
            duration = _info.CutDuration;
        }
        else {
            duration = _info.CutDurationLevel3;
        }

        yield return new WaitForSeconds(duration);

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager enemyHealth)) return;

        if (IsServer) enemyHealth.ApplyDamageOnServerRPC(_info.Damage, true, true);
        
        enemyHealth.AddDebuffToList(_info.BleedingDebuff);

        _father.acumulativeDamage += _info.Damage * _info.PercentOfDamageToAcumulate / 100;
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
