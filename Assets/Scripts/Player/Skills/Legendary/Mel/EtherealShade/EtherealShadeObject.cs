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

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        _collider.radius = _info.InicialRadius;

        gameObject.SetActive(true);

        StartCoroutine(Duration());
        StartCoroutine(HealAndGrow());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.CloneDuration);

        StopCoroutine(HealAndGrow());

        Explode();

        yield return new WaitForSeconds(_info.ExplosionDuration);

        End();
    }

    IEnumerator HealAndGrow() {
        while (true) {
            yield return new WaitForSeconds(_info.HealCooldown);
            bool heal = false;
            foreach(var player in _playersList) {
                player.Heal(_info.Heal, false);
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

        float attackDamage = Mathf.Max(_info.BaseDamage * _amountOfGrowths * (1 + _info.PercentOfDamageIncreasePerGrowth / 100), _info.BaseDamage);
        float damage = _dManager.ReturnTotalAttack(attackDamage);

        foreach(var enemy in _enemiesList) {
            if (enemy == null) {
                _enemiesList.Remove(enemy);
                continue;
            }
            enemy.DealDamage(damage, false, true);
        }
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (other.CompareTag("Enemy") && !_enemiesList.Contains(health)) _enemiesList.Add(health);

        if ((other.CompareTag("Mel") || other.CompareTag("Maevis")) && !_playersList.Contains(health)) _playersList.Add(health);
    }

    private void OnTriggerExit(Collider other) {

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (other.CompareTag("Enemy") && _enemiesList.Contains(health)) _enemiesList.Remove(health);

        if ((other.CompareTag("Mel") || other.CompareTag("Maevis")) && _playersList.Contains(health)) _playersList.Remove(health);
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
    void End() {
        _amountOfGrowths = 0;
        _playersList.Clear();
        _enemiesList.Clear();
        ReturnObject();
    }
}
