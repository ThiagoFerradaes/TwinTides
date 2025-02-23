using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoulSphereArea : SkillObjectPrefab {
    SoulSphere _info;
    int _level;
    SkillContext _context;
    bool _canDamage = true;
    GameObject _mel;

    List<HealthManager> playersList = new();
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

        if (_level == 3) {
            transform.localScale = _info.AreaRadius;

        }
        else if (_level == 4) {
            transform.localScale = _info.AreaRadiusLevel4;
        }


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
            _canDamage = false;
            yield return new WaitForSeconds(_info.AreaDamageCooldown);
            _canDamage = true;
            yield return null;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (!IsServer) return;

        if (!_canDamage) return;

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float damage = _mel.GetComponent<DamageManager>().ReturnTotalAttack(_info.AreaDamage);

        health.ApplyDamageOnServerRPC(damage, true, true);

    }

    private void OnTriggerEnter(Collider other) {

        if (_level < 4) return;

        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (!playersList.Contains(health)) { playersList.Add(health); }

        health.RemoveBuff(_info.invulnerabilityBuff);

        if (IsServer) health.SetPermissionServerRpc(HealthPermissions.CanTakeDamage, false);
    }

    private void OnTriggerExit(Collider other) {

        if (_level < 4) return;

        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (playersList.Contains(health)) { playersList.Remove(health); }

        if (IsServer) health.SetPermissionServerRpc(HealthPermissions.CanTakeDamage, true);

        health.AddBuffToList(_info.invulnerabilityBuff);

    }

    void End() {
        if (IsServer && _level == 4) {
            List<HealthManager> removedPlayers = new(playersList);

            playersList.Clear();

            foreach (var player in removedPlayers) {
                player.SetPermissionServerRpc(HealthPermissions.CanTakeDamage, true);
            }
        }

        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
