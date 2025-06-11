using FMODUnity;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtherealShadeObject : SkillObjectPrefab
{
    EtherealShade _info;
    SkillContext _context;
    GameObject _mel, _healingEffect, _damageEffect;
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

        if (_healingEffect == null) {
            _healingEffect = gameObject.transform.GetChild(1).gameObject;
        }

        if (_damageEffect == null) {
            _damageEffect = gameObject.transform.GetChild(2).gameObject;
        }

        SetPosition();
    }

    void SetPosition() {

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        _collider.radius = _info.InicialRadius;
        _healingEffect.transform.localScale = Vector3.one * _info.InicialRadius;
        _damageEffect.transform.localScale = Vector3.one * _info.InicialRadius;

        gameObject.SetActive(true);

        if (!_info.PositionSound.IsNull) RuntimeManager.PlayOneShot(_info.PositionSound, transform.position);

        StartCoroutine(Duration());
        StartCoroutine(HealAndGrow());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.TotemDuration);

        StopCoroutine(HealAndGrow());

        yield return new WaitForSeconds(_info.HealingEffectDuration);

        Explode();

        yield return new WaitForSeconds(_info.ExplosionDuration);

        ReturnObject();
    }

    IEnumerator HealAndGrow() {
        while (true) {
            yield return new WaitForSeconds(_info.HealCooldown);
            bool heal = false;
            foreach(var player in _playersList) {
                player.Heal(_info.Heal, false);
                heal = true;
            }
            
            StartCoroutine(HealingEffectTimer());
            if (heal) Grow();
        }
    }

    IEnumerator HealingEffectTimer() {
        _healingEffect.SetActive(true);
        if (!_info.HealExplosionSound.IsNull) RuntimeManager.PlayOneShot(_info.HealExplosionSound, transform.position);
        yield return new WaitForSeconds(_info.HealingEffectDuration);
        _healingEffect.SetActive(false);
    }

    IEnumerator DamageEffectTimer() {
        _damageEffect.SetActive(true);
        if (!_info.DamageExplosionSound.IsNull) RuntimeManager.PlayOneShot(_info.DamageExplosionSound, transform.position);
        yield return new WaitForSeconds(_info.ExplosionDuration);
        _damageEffect.SetActive(false);
    }

    void Grow() {
        if (_amountOfGrowths >= _info.MaxAmountOfGrowths) return;
        _amountOfGrowths++;
        _collider.radius *= ( 1 + _info.GrowthPercentage/100);

        _healingEffect.transform.localScale *= (1 + _info.GrowthPercentage / 100);
        _damageEffect.transform.localScale *= (1 + _info.GrowthPercentage / 100);

        if (!_info.GrowthSound.IsNull) RuntimeManager.PlayOneShot(_info.GrowthSound, transform.position );

    }
    void Explode() {

        float attackDamage = Mathf.Max(_info.BaseDamage * _amountOfGrowths * (1 + _info.PercentOfDamageIncreasePerGrowth / 100), _info.BaseDamage);
        float damage = _dManager.ReturnTotalAttack(attackDamage);

        StartCoroutine(DamageEffectTimer());

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
    public override void ReturnObject() {

        _healingEffect.SetActive(false);
        _damageEffect.SetActive(false);

        _amountOfGrowths = 0;
        _playersList.Clear();
        _enemiesList.Clear();
        base.ReturnObject();
    }
}
