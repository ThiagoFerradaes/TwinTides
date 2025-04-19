using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaWardStoneObject : SkillObjectPrefab {
    WardStone _info;
    int _level;
    SkillContext _context;
    GameObject _mel;

    List<HealthManager> _listOfPlayers = new();
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as WardStone;
        _level = skillLevel;
        _context = context;

        if (_mel == null) _mel = PlayerSkillPooling.Instance.MelGameObject;

        DefineSizeAndPosition();

    }

    private void DefineSizeAndPosition() {
        transform.localScale = _info.ExplosionRadiusLevel3;

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(AreaDuration());
        if (_level > 3) StartCoroutine(HealingTimer());
    }

    IEnumerator AreaDuration() {
        float duration = _level < 4 ? _info.AreaDuration : _info.AreaDurationLevel4;
        yield return new WaitForSeconds(duration);

        ReturnObject();
    }

    IEnumerator HealingTimer() {
        while (true) {
            foreach(var player in _listOfPlayers) {
                if (player.ReturnCurrentHealth() >= player.ReturnMaxHealth()) {
                    float shieldAmount = _info.AmountOfHealing * _info.PercentOfShieldFromExtraHealing/100;
                    player.ApplyShield(shieldAmount, _info.ExtraShieldDuration, true);
                }
                else {
                    player.HealServerRpc(_info.AmountOfHealing, true);
                }
            }
            yield return new WaitForSeconds(_info.HealingInterval);
        } 
    }
    private void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;
        
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!_listOfPlayers.Contains(health)) _listOfPlayers.Add(health);

    }

    private void OnTriggerExit(Collider other) {
        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (_listOfPlayers.Contains(health)) _listOfPlayers.Remove(health);
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
