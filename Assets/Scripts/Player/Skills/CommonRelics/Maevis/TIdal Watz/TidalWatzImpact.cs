using FMODUnity;
using System.Collections;
using UnityEngine;

public class TidalWatzImpact : SkillObjectPrefab {
    TidalWatz _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;

    TidalWatzManager _father;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as TidalWatz;
        _level = skillLevel;
        _context = context;
        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        if (_father == null) {
            _father = FindAnyObjectByType<TidalWatzManager>();
        }

        SetSizeAndPosition();
    }

    private void SetSizeAndPosition() {

        Vector3 direction = _father.transform.rotation * Vector3.forward;
        Vector3 position = _father.transform.position + (direction * _info.ImpactOffset);
        transform.SetPositionAndRotation(position, _father.transform.rotation);

        transform.localScale = _info.impactSize;

        gameObject.SetActive(true);

        if (!_info.ImpactSound.IsNull) RuntimeManager.PlayOneShot(_info.ImpactSound, transform.position);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.ImpactDuration);

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float baseDamage = _maevis.GetComponent<DamageManager>().ReturnTotalAttack(_info.BaseDamageImpact);

        float totalDamage = baseDamage + _father.acumulativeDamage;

        health.DealDamage(totalDamage, false, true);
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
