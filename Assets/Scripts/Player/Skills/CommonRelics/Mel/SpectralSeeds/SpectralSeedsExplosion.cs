using System.Collections;
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

        if (_mel == null) _mel = PlayerSkillPooling.Instance.MelGameObject;

        DefineSizeAndPosition();
    }

    private void DefineSizeAndPosition() {
        transform.localScale = _level == 1 ? Vector3.one * _info.ExplosionRadius : Vector3.one * _info.ExplosionRadiusLevel2;

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(ExplosionDuration());
    }

    IEnumerator ExplosionDuration() {
        yield return new WaitForSeconds(_info.ExplosionDuration);

        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float healing = 0;

        if (other.CompareTag("Enemy") && !health.ReturnDeathState()) {
            float damage = _mel.GetComponent<DamageManager>().ReturnTotalAttack(_info.Damage);
            health.DealDamage(damage, true, true);
            healing = damage * _info.PercentOfDamageToHeal / 100;
        }

        if (_level < 3) return;

        if (other.CompareTag("Maevis")) {
            health.HealServerRpc(_info.AmountOfHealToMaevis, true);
        }

        if (_mel.TryGetComponent<HealthManager>(out HealthManager melHealth)) {
            melHealth.HealServerRpc(healing, true);
        }
        
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
