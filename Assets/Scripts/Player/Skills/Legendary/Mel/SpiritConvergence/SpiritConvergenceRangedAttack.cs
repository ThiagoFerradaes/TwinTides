using System.Collections;
using UnityEngine;

public class SpiritConvergenceRangedAttack : SkillObjectPrefab
{
    SpiritConvergence _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    DamageManager _dManager;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as SpiritConvergence;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
            _dManager = _mel.GetComponent<DamageManager>();
        }

        SetPosition();
    }

    void SetPosition() {
        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(Move());
    }

    IEnumerator Move() {
        float elapsedTime = 0f;

        while(elapsedTime < _info.RangedAttackDuration) {
            elapsedTime += Time.deltaTime;
            transform.position += (_info.RangedAttackSpeed * Time.deltaTime * transform.forward);
            yield return null;
        }

        End();
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float damage = _dManager.ReturnTotalAttack(_info.RangedAttackDamage);

        health.DealDamage(damage, true, true);
    }

    void End() {
        ReturnObject();
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
