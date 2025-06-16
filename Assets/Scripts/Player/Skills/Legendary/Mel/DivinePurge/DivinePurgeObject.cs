using FMOD.Studio;
using FMODUnity;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivinePurgeObject : SkillObjectPrefab
{
    DivinePurge _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    PlayerController _mManager;
    HealthManager _hManager;
    DamageManager _dManager;

    List<HealthManager> _enemiesList = new();
    HealthManager _maevisHealth;

    EventInstance sound;

    Animator anim;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as DivinePurge;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
            _mManager = _mel.GetComponent<PlayerController>();
            _hManager = _mel.GetComponent<HealthManager>();
            _dManager = _mel.GetComponent<DamageManager>();
            anim = _mel.GetComponentInChildren<Animator>();
        }

        DefinePosition();
    }

    private void DefinePosition() {
        transform.localScale = _info.SkillSize;

        Vector3 direction = _context.PlayerRotation * Vector3.forward;
        Vector3 position = _context.Pos + (direction * (_info.ZOffSett + _info.SkillSize.y/2));
        transform.SetPositionAndRotation(position, _context.PlayerRotation * Quaternion.Euler(90,0,0));

        gameObject.SetActive(true);

        if (_info.animationName != null) anim.SetBool(_info.animationName, true);

        if (!_info.LaserSound.IsNull) {
            sound = RuntimeManager.CreateInstance(_info.LaserSound);
            RuntimeManager.AttachInstanceToGameObject(sound, this.gameObject);
            sound.start();
        }

        StartCoroutine(SkillDuration());

        StartCoroutine(DamageCoroutine());
    }

    IEnumerator SkillDuration() {
        _mManager.BlockMovement();

        yield return new WaitForSeconds(_info.Duration);

        if (_info.animationName != null) anim.SetBool(_info.animationName, false);

        ReturnObject();
    }

    public override void ReturnObject() {
        _mManager.AllowMovement();

        _maevisHealth = null;
        _enemiesList.Clear();

        if (sound.isValid()) {
            sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            sound.release();
        }

        base.ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (other.CompareTag("Maevis")) _maevisHealth = health;

        else if (other.CompareTag("Enemy") && !_enemiesList.Contains(health)) _enemiesList.Add(health); 
    }

    private void OnTriggerExit(Collider other) {

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (other.CompareTag("Maevis")) _maevisHealth = null;

        else if (other.CompareTag("Enemy") && _enemiesList.Contains(health)) _enemiesList.Remove(health);
    }

    IEnumerator DamageCoroutine() {
        float elapsedTime = 0f;

        while (elapsedTime < _info.Duration) {
            yield return new WaitForSeconds(_info.DamageCooldown);
            float damage = _dManager.ReturnTotalAttack(_info.DamagePerTick);
            float totalDamage = 0f;
            foreach (var enemy in _enemiesList) {
                if (!enemy.ReturnDeathState()) {
                    enemy.DealDamage(damage, true, true);
                    totalDamage += damage;
                }
            }
            _hManager.Heal(totalDamage * _info.PercentOfHealingBasedOnDamage/100, true);
            if(_maevisHealth != null)_maevisHealth.Heal(_info.AmountOfHealToMaevis, true);
        }
    }
}
