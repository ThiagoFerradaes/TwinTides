using System.Collections;
using UnityEngine;

public class MaevisNormalAttackObject : SkillObjectPrefab {
    MaevisNormalAttack _info;
    int _currentAttackCombo;
    SkillContext _context;
    GameObject _maevis;
    MaevisNormalAttackManager _father;
    DamageManager _dManager;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as MaevisNormalAttack;
        _currentAttackCombo = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            _dManager = _maevis.GetComponent<DamageManager>();
        }
        if (_father == null) {
            _father = GameObject.FindAnyObjectByType<MaevisNormalAttackManager>();
        }

        _father.OnEndOfAttack -= _father_OnEndOfAttack;
        _father.OnEndOfAttack += _father_OnEndOfAttack; 

        DefinePosition();
    }

    private void _father_OnEndOfAttack(object sender, System.EventArgs e) {
        End();
    }

    void DefinePosition() { 
        transform.localScale = _currentAttackCombo == 3 ? _info.ThirdAttackSize : _info.FirstAndSecondAttackSize;

        transform.SetParent(_father.transform);

        transform.localPosition = _currentAttackCombo == 3 ? _info.ThirdAttackPosition : _info.AttackPosition;
        transform.localRotation = _currentAttackCombo == 3 ? Quaternion.Euler(_info.ThierdAttackRotation) : Quaternion.Euler(_info.AttackRotation);


        gameObject.SetActive(true);
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float damage = _currentAttackCombo switch {
            1 => _dManager.ReturnTotalAttack(_info.FirstAttackDamage),
            2 => _dManager.ReturnTotalAttack(_info.SecondAttackDamage),
            _ => _dManager.ReturnTotalAttack(_info.ThirdAttackDamage)
        };

        health.DealDamage(damage, true, true);
    }

    void End() {
        _currentAttackCombo = 1;

        _father.OnEndOfAttack -= _father_OnEndOfAttack;

        ReturnObject();
    }
}
