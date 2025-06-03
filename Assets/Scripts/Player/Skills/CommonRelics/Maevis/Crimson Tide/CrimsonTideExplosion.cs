using FMODUnity;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CrimsonTideExplosion : SkillObjectPrefab {
    CrimsonTide _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as CrimsonTide;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        DefineSizeAndPosition();
    }

    private void DefineSizeAndPosition() {
        transform.localScale = _info.ExplosionRadius * Vector3.one;

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        if (!_info.ExplosionSound.IsNull) RuntimeManager.PlayOneShot(_info.ExplosionSound);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.ExplosionDuration);

        End();
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        bool shouldExecute = health.ReturnCurrentHealth() <= health.ReturnMaxHealth() * _info.PercentToExecute / 100 && _level >= 4;
        float damage = shouldExecute ? 9999 : _maevis.GetComponent<DamageManager>().ReturnTotalAttack(_info.ExplosionDamage);

        bool wasAlive = !health.ReturnDeathState();

        health.DealDamage(damage, true, true);

        bool isDead = health.ReturnDeathState();

        if (_level == 4 && wasAlive && isDead) {
            Health_OnDeath();
        }
    }

    private void Health_OnDeath() {
        if (_info.Character == LocalWhiteBoard.Instance.PlayerCharacter)
            _maevis.GetComponent<PlayerSkillManager>().ResetCooldown(_context.SkillIdInUI);
    }

    void End() {

        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
