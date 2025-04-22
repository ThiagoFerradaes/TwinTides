using System;
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SacrificeObject : SkillObjectPrefab {

    Sacrifice _info;
    int _level;
    SkillContext _context;
    GameObject _mel;

    NetworkVariable<bool> _healedMaevis = new(false);

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Sacrifice;
        _level = skillLevel;
        _context = context;

        if (_mel == null) _mel = PlayerSkillPooling.Instance.MelGameObject;

        DefinePosition();
    }
    void DefinePosition() {
        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        _healedMaevis.Value = false;

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

        ReturnObject();
    }

    void DrainHealth() {

        if (!_mel.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float healthLost = health.ReturnMaxHealth() * _info.HealthLostPercent / 100;

        health.DealDamage(healthLost, false, false);

    }
    void RecoverHealth() {

        if (!_mel.TryGetComponent<HealthManager>(out HealthManager health)) return;
        var healthGain = _level switch {
            1 => health.ReturnMaxHealth() * _info.HealthGainPercent / 100,
            2 => health.ReturnMaxHealth() * _info.HealthGainPercentLevel2 / 100,
            _ => health.ReturnMaxHealth() * _info.HealthGainPercentLevel3 / 100,
        };
        health.Heal(healthGain, false);

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
        if (!other.CompareTag("Maevis") || _healedMaevis.Value) return;
        if (!other.TryGetComponent<HealthManager>(out HealthManager healthManager)) return;

       HealMaevis(healthManager);

        ApllyBuffToMaevis(healthManager);

    }
    void HealMaevis(HealthManager maevisHealthM) {
        float healthGain = _level switch {
            1 => _info.Healing,
            _ => _info.HealingLevel2,
        };

        maevisHealthM.Heal(healthGain, true);

        if (_level == 4) {
            maevisHealthM.CleanAllDebuffsRpc();
        }

        _healedMaevis.Value = true;
    }

    void ApllyBuffToMaevis(HealthManager maevisHealthM) {
        if (_level >= 3) {
            maevisHealthM.AddBuffToList(_info.HealingOverTimeBuff);
        }
    }
}
