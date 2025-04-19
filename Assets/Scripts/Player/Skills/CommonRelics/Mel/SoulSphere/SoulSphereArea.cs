using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoulSphereArea : SkillObjectPrefab {
    SoulSphere _info;
    int _level;
    SkillContext _context;
    GameObject _mel;

    List<HealthManager> playersList = new();
    List<HealthManager> _enemiesList = new();
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as SoulSphere;
        _level = skillLevel;
        _context = context;

        if(_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
        }

        DefineSize();

        StartCoroutine(Timer());

        StartCoroutine(DamageCooldown());
    }

    void DefineSize() {

        _context.PlayerPosition.y = GetGroundHeith(_context.PlayerPosition);

        transform.SetPositionAndRotation(_context.PlayerPosition, _context.PlayerRotation);

        transform.localScale = _level < 4? _info.AreaRadius : _info.AreaRadiusLevel4;

        gameObject.SetActive(true);
    }

    float GetGroundHeith(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) return hit.point.y + 0.1f;
        return position.y;
    }

    IEnumerator Timer() {
        float duration = _level < 4 ? _info.AreaDurationLevel3 : _info.AreaDurationLevel4;
        yield return new WaitForSeconds(duration);

        End();
    }

    IEnumerator DamageCooldown() {
        while (true) {
            yield return new WaitForSeconds(_info.AreaDamageCooldown);
            foreach (var enemy in _enemiesList) {
                float damage = _mel.GetComponent<DamageManager>().ReturnTotalAttack(_info.AreaDamage);
                enemy.DealDamage(damage, true, true);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {

        if (_level < 4) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (other.CompareTag("Enemy")) {
            if (!_enemiesList.Contains(health)) _enemiesList.Add(health);
        }

        if (other.CompareTag("Mel") || other.CompareTag("Maevis")) {
            if (!playersList.Contains(health)) { playersList.Add(health); }

            health.RemoveBuff(_info.invulnerabilityBuff);

            health.SetPermissionServerRpc(HealthPermissions.CanTakeDamage, false);
        }
    }

    private void OnTriggerExit(Collider other) {

        if (_level < 4) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (other.CompareTag("Enemy")) {
            if (_enemiesList.Contains(health)) _enemiesList.Remove(health);
        }

        if (other.CompareTag("Mel") || other.CompareTag("Maevis")) {

            if (playersList.Contains(health)) { playersList.Remove(health); }

            health.SetPermissionServerRpc(HealthPermissions.CanTakeDamage, true);

            health.AddBuffToList(_info.invulnerabilityBuff);
        }

    }

    void End() {
        if (_level == 4) {
            List<HealthManager> removedPlayers = new(playersList);

            playersList.Clear();

            foreach (var player in removedPlayers) {
                player.SetPermissionServerRpc(HealthPermissions.CanTakeDamage, true);
            }

            _enemiesList.Clear();
        }

        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
