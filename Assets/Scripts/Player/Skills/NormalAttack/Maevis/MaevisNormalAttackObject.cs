using FMODUnity;
using System.Collections;
using UnityEngine;

public class MaevisNormalAttackObject : SkillObjectPrefab {
    MaevisNormalAttack _info;
    int _currentAttackCombo;
    GameObject _maevis;
    MaevisNormalAttackManager _father;
    DamageManager _dManager;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as MaevisNormalAttack;
        _currentAttackCombo = skillLevel;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            _dManager = _maevis.GetComponent<DamageManager>();
        }
        if (_father == null) {
            _father = GameObject.FindAnyObjectByType<MaevisNormalAttackManager>();
        }

        _father.OnEndOfAttack -= Father_OnEndOfAttack;
        _father.OnEndOfAttack += Father_OnEndOfAttack; 

        DefinePosition();
    }

    private void Father_OnEndOfAttack(object sender, System.EventArgs e) {
        End();
    }

    void DefinePosition() {
        BoxCollider col = GetComponent<BoxCollider>();
        col.size = _currentAttackCombo == 3 ? _info.ThirdAttackSize : _info.FirstAndSecondAttackSize;

        transform.SetParent(_father.transform);

        transform.localPosition = _currentAttackCombo == 3 ? Vector3.forward * _info.ThirdAttackPosition : Vector3.forward * _info.AttackPosition;

        transform.localRotation = Quaternion.Euler(0, 0, 0);

        transform.GetChild(0).gameObject.SetActive(_currentAttackCombo == 1);
        transform.GetChild(1).gameObject.SetActive(_currentAttackCombo == 2);
        transform.GetChild(2).gameObject.SetActive(_currentAttackCombo == 3);

        gameObject.SetActive(true);

        RuntimeManager.PlayOneShot(_info.attackSound, transform.position);
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Enemy") && !other.CompareTag("BlackBeardBomb")) return;

        if (other.TryGetComponent<BlackBeardCannonBomb>(out var bomb)) {
            bomb.TryPush();
            return;
        }

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

        _father.OnEndOfAttack -= Father_OnEndOfAttack;

        ReturnObject();
    }
}
