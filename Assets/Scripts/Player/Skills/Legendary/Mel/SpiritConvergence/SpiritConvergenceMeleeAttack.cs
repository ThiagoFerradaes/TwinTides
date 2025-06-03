using FMODUnity;
using System.Collections;
using UnityEngine;

public class SpiritConvergenceMeleeAttack : SkillObjectPrefab
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
        Vector3 direction = _context.PlayerRotation * Vector3.forward;
        Vector3 position = _context.Pos + (direction * _info.MeleeAttackOffSet);
        transform.SetPositionAndRotation(position, _context.PlayerRotation);

        gameObject.SetActive(true);

        if (!_info.MeleeMinionHitSound.IsNull) RuntimeManager.PlayOneShot(_info.MeleeMinionHitSound);   

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.MeleeAttackDuration);

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float damage = _dManager.ReturnTotalAttack(_info.MeleeAttackDamage);

        health.DealDamage(damage, true, true);
    }

    void End() {
        ReturnObject();
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
