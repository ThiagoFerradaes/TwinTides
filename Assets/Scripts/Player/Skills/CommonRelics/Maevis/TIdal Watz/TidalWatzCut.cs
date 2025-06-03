using FMODUnity;
using System;
using System.Collections;
using UnityEngine;

public class TidalWatzCut : SkillObjectPrefab {
    TidalWatz _info;
    int _level;
    GameObject _maevis;
    TidalWatzObject _father;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as TidalWatz;
        _level = skillLevel;
        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        SetSizeAndPosition();
    }

    private void SetSizeAndPosition() {
        if (_father == null) {
            _father = GameObject.FindAnyObjectByType<TidalWatzObject>();
        }

        transform.localScale = _level == 1 ? Vector3.one * _info.CutSize : Vector3.one * _info.CutSizeLevel2;

        transform.SetParent(_father.transform);

        Vector3 position = Vector3.zero;

        transform.SetLocalPositionAndRotation(position, Quaternion.Euler(0, 0, 0));

        gameObject.SetActive(true);

        if (_level < 3) {
            if (!_info.CutSound.IsNull) RuntimeManager.PlayOneShot(_info.CutSound);
        }
        else {
            if (!_info.CutSoundFaster.IsNull) RuntimeManager.PlayOneShot(_info.CutSoundFaster);
        }

            StartCoroutine(CutDuration());
    }

    IEnumerator CutDuration() {
        float duration = _level < 3 ? _info.CutDuration : _info.CutDurationLevel3;

        yield return new WaitForSeconds(duration);

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager enemyHealth)) return;

        float damage = _maevis.GetComponent<DamageManager>().ReturnTotalAttack(_info.Damage);

        enemyHealth.DealDamage(_info.Damage, true, true);

        enemyHealth.AddDebuffToList(_info.BleedingDebuff);

        _father.acumulativeDamage += damage * _info.PercentOfDamageToAcumulate / 100;
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
