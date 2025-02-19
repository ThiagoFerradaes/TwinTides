using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SacrificeObject : SkillObjectPrefab {

    Sacrifice _info;
    int _level;
    SkillContext _context;
    GameObject _mel;

    bool _healedMaevis;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Sacrifice;
        _level = skillLevel;
        _context = context;

        DefinePosition();
    }
    void FindMel() {
        if (_mel != null) return;
        _mel = GameObject.FindGameObjectWithTag("Mel");
    }
    void DefinePosition() {
        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        _healedMaevis = false;

        FindMel();

        StartCoroutine(Move());

        DrainHealth();
    }

    IEnumerator Move() {
        float inicialTime = Time.time;

        while (Time.time - inicialTime < _info.Duration) {
            transform.Translate(_info.SphereSpeed * Time.deltaTime * Vector3.forward);
            yield return null;
        }

        while (Vector3.Distance(transform.position, _mel.transform.position) > 0.2f) {
            Vector3 direction = (_mel.transform.position - transform.position).normalized;
            transform.position += (_info.SphereBackSpeed * Time.deltaTime * direction);
            yield return null;
        }

        RecoverHealth();

        ApllyBuffToMel();

        Cooldown();

        ReturnObject();
    }

    void DrainHealth() {
        if (!_mel.TryGetComponent<HealthManager>(out HealthManager health)) return;
        if (!IsServer) return;

        float healthLost = health.maxHealth.Value * _info.HealthLostPercent / 100;

        health.ApplyDamageOnServerRPC(healthLost, false, false);

    }
    void RecoverHealth() {
        if (!_mel.TryGetComponent<HealthManager>(out HealthManager health)) return;
        if (!IsServer) return;
        var healthGain = _level switch {
            1 => health.maxHealth.Value * _info.HealthGainPercent / 100,
            2 => health.maxHealth.Value * _info.HealthGainPercentLevel2 / 100,
            _ => health.maxHealth.Value * _info.HealthGainPercentLevel3 / 100,
        };
        health.HealServerRpc(healthGain);

        if (_level == 4) {
            health.CleanAllDebuffsRpc();
        }
    }

    void ApllyBuffToMel() {
        if (_level >= 3 && _mel.TryGetComponent<HealthManager>(out HealthManager health)) {
            health.AddBuffToList(_info.HealingOverTimeBuff);
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Maevis") || _healedMaevis) return;
        if (!other.TryGetComponent<HealthManager>(out HealthManager healthManager)) return;

        if (IsServer) HealMaevis(healthManager);

        ApllyBuffToMaevis(healthManager);

    }
    void HealMaevis(HealthManager maevisHealthM) {
        float healthGain = _level switch {
            1 => _info.Healing,
            _ => _info.HealingLevel2,
        };

        maevisHealthM.HealServerRpc(healthGain);
        _healedMaevis = true;

        if (_level == 4) {
            maevisHealthM.CleanAllDebuffsRpc();
        }
    }

    void ApllyBuffToMaevis(HealthManager maevisHealthM) {
        if (_level >= 3) {
            maevisHealthM.AddBuffToList(_info.HealingOverTimeBuff);
        }
    }

    void Cooldown() {
        _mel.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, _info);
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
