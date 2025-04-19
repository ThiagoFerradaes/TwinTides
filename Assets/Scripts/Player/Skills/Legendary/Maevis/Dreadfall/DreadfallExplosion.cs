using System.Collections;
using UnityEngine;

public class DreadfallExplosion : SkillObjectPrefab {
    Dreadfall _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    DamageManager _dManager;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Dreadfall;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            _dManager = _maevis.GetComponent<DamageManager>();
        }

        SetPosition();
    }

    void SetPosition() {
        transform.localScale = Vector3.one * _info.ExplosionRadius;

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.ExplosionDuration);

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 2);


        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float damage = _dManager.ReturnTotalAttack(_info.ExplosionDamage);

        health.DealDamage(damage, true, true);
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
