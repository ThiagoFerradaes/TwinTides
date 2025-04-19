using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleObject : SkillObjectPrefab {
    BlackHole _info;
    int _level;
    SkillContext _context;
    GameObject _mel;

    List<HealthManager> _listOfEnemies = new();
    List<MovementManager> _listOfMEnemies = new();

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as BlackHole;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
        }

        DefineSizeAndPosition();
    }

    private void DefineSizeAndPosition() {
        transform.localScale = _level < 4 ? _info.Size : _info.SizeLevel4;

        Transform aim = _mel.GetComponent<PlayerController>().aimObject;

        _context.Pos.y = GetGroundHeight(_context.Pos);

        Vector3 direction = _context.PlayerRotation * Vector3.forward;
        Vector3 position = _context.Pos + (direction * _info.MaxRange);

        if (aim != null && aim.gameObject.activeInHierarchy && Vector3.Distance(_context.Pos, aim.position) <= _info.MaxRange) {
            transform.SetPositionAndRotation(new Vector3(aim.position.x, _context.Pos.y, aim.position.z), _context.PlayerRotation);
        }
        else {
            transform.SetPositionAndRotation(position, _context.PlayerRotation);
        }

        gameObject.SetActive(true);

        StartCoroutine(DurationOfBlackHole());

        StartCoroutine(DamageTimer());
        if (_level > 1) StartCoroutine(StunTimer());

    }

    float GetGroundHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) {
            return hit.point.y + 0.1f;
        }
        return position.y;
    }

    IEnumerator DurationOfBlackHole() {
        float time = _level < 3 ? _info.Duration : _info.DurationLevel3;

        yield return new WaitForSeconds(time);

        End();
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent<MovementManager>(out MovementManager mManager)) {
            if (!_listOfMEnemies.Contains(mManager)) _listOfMEnemies.Add(mManager);
        }
        if (other.TryGetComponent<HealthManager>(out HealthManager health)) {
            if (!_listOfEnemies.Contains(health)) _listOfEnemies.Add(health);
        }

        mManager.DecreaseMoveSpeedRpc(_info.SlowPercent);

        if (_level < 2) return;

        if (health.ReturnShieldStatus()) {
            health.BreakShieldRpc();
        }

        if (_level < 3) return;

        health.SetMultiplyServerRpc(HealthMultipliers.Heal, (1 - _info.HealReductionPercent / 100));
        health.SetPermissionServerRpc(HealthPermissions.CanBeShielded, false);

    }

    private void OnTriggerExit(Collider other) {

        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent<MovementManager>(out MovementManager mManager)) {
            if (_listOfMEnemies.Contains(mManager)) _listOfMEnemies.Remove(mManager);
        }
        if (other.TryGetComponent<HealthManager>(out HealthManager health)) {
            if (_listOfEnemies.Contains(health)) _listOfEnemies.Remove(health);
        }

        mManager.IncreaseMoveSpeedRpc(_info.SlowPercent);

        if (_level < 3) return;

        health.SetMultiplyServerRpc(HealthMultipliers.Heal, (1 / (1 - _info.HealReductionPercent / 100)));
        health.SetPermissionServerRpc(HealthPermissions.CanBeShielded, true);

    }

    IEnumerator DamageTimer() {
        while (true) {
            yield return new WaitForSeconds(_info.DamageInterval);
            foreach (var enemy in _listOfEnemies) {
                float damage = _mel.GetComponent<DamageManager>().ReturnTotalAttack(_info.Damage);
                enemy.DealDamage(damage, false, true);
            }
        }
    }

    IEnumerator StunTimer() {
        while (true) {
            yield return new WaitForSeconds(_info.StunInterval);
            foreach (var enemy in _listOfMEnemies) {
                if (_level < 4) enemy.GetComponent<MovementManager>().StunWithTimeRpc(_info.StunDuration);
                else enemy.GetComponent<MovementManager>().StunWithTimeRpc(_info.StunDurationLevel4);
            }
        }
    }

    void End() {

        List<MovementManager> removed = new(_listOfMEnemies);
        _listOfMEnemies.Clear();

        foreach (var enemy in removed) {
            enemy.IncreaseMoveSpeedRpc(_info.SlowPercent);
        }

        if (_level == 4) {
            List<HealthManager> hRemoved = new(_listOfEnemies);
            _listOfEnemies.Clear();

            foreach (var enemy in hRemoved) {
                enemy.SetMultiplyServerRpc(HealthMultipliers.Heal, (1 / (1 - _info.HealReductionPercent / 100)));
                enemy.SetPermissionServerRpc(HealthPermissions.CanBeShielded, true);
            }
        }

        _listOfEnemies.Clear();

        ReturnObject();
    }
}
