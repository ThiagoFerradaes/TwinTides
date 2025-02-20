using System.Collections;
using UnityEngine;

public class MaevisNormalAttackObject : SkillObjectPrefab {
    MaevisNormalAttack _info;
    int _currentAttackCombo;
    SkillContext _context;
    GameObject _maevis;
    GameObject _father;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as MaevisNormalAttack;
        _currentAttackCombo = skillLevel;
        _context = context;

        DefineParentAndPosition();
    }

    void DefineParentAndPosition() {
        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }
        if (_father == null) {
            _father = GameObject.FindAnyObjectByType<MaevisNormalAttackManager>().gameObject;
        }

        transform.SetParent(_father.transform);

        transform.localPosition = _currentAttackCombo == 3 ? _info.ThirdAttackPosition : _info.AttackPosition;
        transform.localRotation = _currentAttackCombo == 3 ? Quaternion.Euler(_info.ThierdAttackRotation) : Quaternion.Euler(_info.AttackRotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        float elapsedTime = 0;
        float duration = _currentAttackCombo switch {
            1 => _info.DurationOfFirstAttack,
            2 => _info.DurationOfSecondAttack,
            _ => _info.DurationOfThirdAtack
        };

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float damage = _currentAttackCombo switch {
            1 =>  _info.FirstAttackDamage,
            2 => _info.SecondAttackDamage,
            _ => _info.ThirdAttackDamage
        };

        health.ApplyDamageOnServerRPC(damage, true, true);
    }
}
