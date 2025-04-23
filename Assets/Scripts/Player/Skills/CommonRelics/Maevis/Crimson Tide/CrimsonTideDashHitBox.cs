using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CrimsonTideDashHitBox : SkillObjectPrefab {
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

        SetParent();
    }

    private void SetParent() {

        transform.SetParent(_maevis.transform);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);


        gameObject.SetActive(true);
        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.DashDuration);

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
            Health_OnDeathRpc();
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void Health_OnDeathRpc() {
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
