using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleObject : SkillObjectPrefab {
    BlackHole _info;
    int _level;
    SkillContext _context;
    List<GameObject> _enemies = new();
    GameObject _mel;

    bool _canDealDamage;
    bool _canStun;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as BlackHole;
        _level = skillLevel;
        _context = context;

        DefineSizeAndPosition();
    }

    private void DefineSizeAndPosition() {
        if (_mel == null) { // Pegando referencia do objeto do jogador
            _mel = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        if (_level < 4) { // Definindo o tamanho da habilidade
            transform.localScale = _info.Size;
        }
        else {
            transform.localScale = _info.SizeLevel4;
        }

        Transform aim = _mel.GetComponent<PlayerController>().aimObject;

        Vector3 direction = _context.PlayerRotation * Vector3.forward;
        Vector3 position = _context.PlayerPosition + (direction * _info.MaxRange);

        if (aim != null && aim.gameObject.activeInHierarchy && Vector3.Distance(_context.PlayerPosition, aim.position) <= _info.MaxRange) {
            transform.SetPositionAndRotation(aim.position, _context.PlayerRotation);
        }
        else {
            transform.SetPositionAndRotation(position, _context.PlayerRotation);
        }

        gameObject.SetActive(true);

        StartCoroutine(DurationOfBlackHole());
        StartCoroutine(DamageTimer());
        StartCoroutine(StunTimer());
    }

    IEnumerator DurationOfBlackHole() {
        float time;
        if (_level < 3) {
            time = _info.Duration;
        }
        else {
            time = _info.DurationLevel3;
        }

        yield return new WaitForSeconds(time);

        ReturnObject();
    }

    private void OnTriggerStay(Collider other) {
        if (!IsServer) return;
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent<HealthManager>(out HealthManager enemyHealth)) {
            if (_canDealDamage) {
                enemyHealth.ApplyDamageOnServerRPC(_info.Damage, true, true);
            }

            if (_level == 4) {
                enemyHealth.SetPermissionServerRpc(HealthPermissions.CanBeShielded, false);
            }
        }

        if (_canStun) {
            Debug.Log("Stun");
        }
        // APLICAR SLOW
        // APLICAR STUN


    }

    private void OnTriggerEnter(Collider other) {
        if (_level < 3 || !IsServer || other.TryGetComponent<HealthManager>(out HealthManager enemyHealth) || !other.CompareTag("Enemy")) return;

        if (enemyHealth.isShielded.Value) {
            float shieldHealth = enemyHealth.currentShieldAmount.Value;
            enemyHealth.ApplyDamageOnServerRPC(shieldHealth, true, false);
        }

        _enemies.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other) {
        if (!IsServer) return;
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent<HealthManager>(out HealthManager enemyHealth)) {
            if (_level == 4) {
                enemyHealth.SetPermissionServerRpc(HealthPermissions.CanBeShielded, true);
            }
        }

        _enemies.Remove(other.gameObject);
    }

    IEnumerator DamageTimer() {
        _canDealDamage = false;
        yield return new WaitForSeconds(_info.DamageInterval);
        _canDealDamage = true;
        yield return null;
        StartCoroutine(DamageTimer());
    }

    IEnumerator StunTimer() {
        _canStun = false;
        yield return new WaitForSeconds(_info.StunInterval);
        _canStun = true;
        StartCoroutine(StunTimer());
    }

    private void OnDisable() {
        if (!IsServer || _enemies.Count == 0) return;

        foreach (var other in _enemies) {
            if (other.TryGetComponent(out HealthManager enemyHealth)) {
                enemyHealth.SetPermissionServerRpc(HealthPermissions.CanBeShielded, true);
            }
        }

        _enemies.Clear();
    }
}
