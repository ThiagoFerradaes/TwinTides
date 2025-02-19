using System.Collections;
using UnityEngine;

public class SoulSphereArea : SkillObjectPrefab {
    SoulSphere _info;
    int _level;
    SkillContext _context;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as SoulSphere;
        _level = skillLevel;
        _context = context;

        DefineSize();

        StartCoroutine(Timer());
    }

    void DefineSize() {
        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        if (_level == 3) {
            transform.localScale = _info.AreaRadius;

            gameObject.SetActive(true);
        }
        else if (_level == 4) {
            transform.localScale = _info.AreaRadiusLevel4;

            gameObject.SetActive(true);
        }
    }
    IEnumerator Timer() {        
        if (_level == 3) {
            yield return new WaitForSeconds(_info.AreaDurationLevel3);

        }
        else if (_level == 4) {
            yield return new WaitForSeconds(_info.AreaDurationLevel4);
        }

        ReturnObject();
    }

    private void OnTriggerStay(Collider other) {
        if (!other.CompareTag("Maevis") || !other.CompareTag("Mel")) return;

        if (other.TryGetComponent<HealthManager>(out HealthManager health)) {
            health.SetPermissionServerRpc(HealthPermissions.CanTakeDamage, false);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Maevis") || !other.CompareTag("Mel")) return;

        if (other.TryGetComponent<HealthManager>(out HealthManager health)) {
            health.SetPermissionServerRpc(HealthPermissions.CanTakeDamage, true);
        }
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
