using System;
using System.Collections;
using TMPro.EditorUtilities;
using UnityEngine;

public class SpectralSeedsExplosion : SkillObjectPrefab {
    SpectralSeeds _info;
    int _level;
    SkillContext _context;

    GameObject _mel;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as SpectralSeeds;
        _level = skillLevel;
        _context = context;

        DefineSizeAndPosition();
    }

    private void DefineSizeAndPosition() {
        if (_mel == null) {
            _mel = GameObject.FindGameObjectWithTag("Mel");
        }

        if (_level == 1) {
            transform.localScale = Vector3.one * _info.ExplosionRadius;
        }
        else {
            transform.localScale = Vector3.one * _info.ExplosionRadiusLevel2;
        }

        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(ExplosionDuration());
    }

    IEnumerator ExplosionDuration() {
        yield return new WaitForSeconds(_info.ExplosionDuration);

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;
        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float healing = 0;

        if (other.CompareTag("Enemy")) {
            health.ApplyDamageOnServerRPC(_info.Damage, true, true);
            healing = _info.PercentOfDamageToHeal / 100 * _info.Damage;
        }

        if (_level < 3) return;

        if (other.CompareTag("Maevis")) {
            health.HealServerRpc(_info.AmountOfHealToMaevis);
        }

        if (_mel.TryGetComponent<HealthManager>(out HealthManager melHealth)) {
            melHealth.HealServerRpc(healing);
        }
        
    }
}
