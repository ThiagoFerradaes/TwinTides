using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtherealShadeObject : SkillObjectPrefab
{
    EtherealShade _info;
    SkillContext _context;
    GameObject _mel;
    DamageManager _dManager;
    SphereCollider _collider;
    int _amountOfGrowths;

    List<HealthManager> _enemiesList = new();
    List<HealthManager> _playersList = new();
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as EtherealShade;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
            _dManager = _mel.GetComponent<DamageManager>();
            _collider = GetComponent<SphereCollider>();
        }

        SetPosition();
    }

    void SetPosition() {
        Transform aim = _mel.GetComponent<PlayerController>().aimObject;

        _context.PlayerPosition.y = GetGroundHeight(_context.PlayerPosition);

        Vector3 direction = _context.PlayerRotation * Vector3.forward;
        Vector3 position = _context.PlayerPosition + (direction * _info.MaxRangeToPlace);

        if (aim != null && aim.gameObject.activeInHierarchy && Vector3.Distance(_context.PlayerPosition, aim.position) <= _info.MaxRangeToPlace) {
            transform.SetPositionAndRotation(new Vector3(aim.position.x, _context.PlayerPosition.y, aim.position.z), _context.PlayerRotation);
        }
        else {
            transform.SetPositionAndRotation(position, _context.PlayerRotation);
        }

        _collider.radius = _info.InicialRadius;

        gameObject.SetActive(true);

        StartCoroutine(Duration());
        if (IsServer) StartCoroutine(HealAndGrow());
    }

    float GetGroundHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) {
            return hit.point.y + 0.1f;
        }
        return position.y;
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.CloneDuration);

        if (IsServer) StopCoroutine(HealAndGrow());

        Explode();

        yield return new WaitForSeconds(_info.ExplosionDuration);

        End();
    }

    IEnumerator HealAndGrow() {
        while (true) {
            yield return new WaitForSeconds(_info.HealCooldown);
            bool heal = false;
            foreach(var player in _playersList) {
                player.HealServerRpc(_info.Heal);
                heal = true;
            }
            if (heal) Grow();
        }
    }

    void Grow() {
        if (_amountOfGrowths >= _info.MaxAmountOfGrowths) return;
        _amountOfGrowths++;
        _collider.radius *= ( 1 + _info.GrowthPercentage/100);
    }
    void Explode() {
        if (!IsServer) return;
        float attackDamage = Mathf.Max(_info.BaseDamage * _amountOfGrowths * (1 + _info.PercentOfDamageIncreasePerGrowth / 100), _info.BaseDamage);
        float damage = _dManager.ReturnTotalAttack(attackDamage);

        foreach(var enemy in _enemiesList) {
            if (enemy == null) {
                _enemiesList.Remove(enemy);
                continue;
            }
            enemy.ApplyDamageOnServerRPC(damage, false, true);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (other.CompareTag("Enemy") && !_enemiesList.Contains(health)) _enemiesList.Add(health);

        if ((other.CompareTag("Mel") || other.CompareTag("Maevis")) && !_playersList.Contains(health)) _playersList.Add(health);
    }

    private void OnTriggerExit(Collider other) {
        if (!IsServer) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (other.CompareTag("Enemy") && _enemiesList.Contains(health)) _enemiesList.Remove(health);

        if ((other.CompareTag("Mel") || other.CompareTag("Maevis")) && _playersList.Contains(health)) _playersList.Remove(health);
    }
    void End() {
        _amountOfGrowths = 0;
        _playersList.Clear();
        _enemiesList.Clear();
        ReturnObject();
    }
}
