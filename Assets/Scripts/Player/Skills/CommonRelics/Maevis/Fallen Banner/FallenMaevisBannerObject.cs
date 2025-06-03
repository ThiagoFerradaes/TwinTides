using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FallenMaevisBannerObject : SkillObjectPrefab {
    FallenBanner _info;
    int _level;
    SkillContext _context;
    GameObject _maveis;
    DamageManager _damageManager;
    int _amountOfBuffs;

    List<GameObject> activePlayers = new();
    Coroutine durationCoroutine;
    public static event System.EventHandler OnStacked;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as FallenBanner;
        _level = skillLevel;
        _context = context;

        if (_maveis == null) {
            _maveis = PlayerSkillPooling.Instance.MaevisGameObject;
            _damageManager = _maveis.GetComponent<DamageManager>();
        }

        InvocateBanner();
    }

    private void InvocateBanner() {
        if (_level < 2) {

            transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

            gameObject.SetActive(true);

            if (!_info.BannerFallingSound.IsNull) RuntimeManager.PlayOneShot(_info.BannerFallingSound);

            durationCoroutine = StartCoroutine(BannerDuration());
        }
        else {

            transform.SetParent(_maveis.transform);

            transform.SetLocalPositionAndRotation(_info.BannerFollowPosition, Quaternion.Euler(0, 0, 0));

            gameObject.SetActive(true);

            if (!_info.BannerSound.IsNull) RuntimeManager.PlayOneShot(_info.BannerSound);

            AddBuffs();
        }
    }

    IEnumerator BannerDuration() {
        float duration = _level < 4 ? _info.BannerDuration : _info.BannerDurationLevel4;

        yield return new WaitForSeconds(duration);

        durationCoroutine = null;

        End();
    }

    void AddBuffs() {

        if (_amountOfBuffs < _info.BannerMaxStacks) {
            _amountOfBuffs++;
            _damageManager.IncreaseBaseAttack(_info.BaseAttackIncreaseLevel2);
        }

        if (durationCoroutine != null) {
            StopCoroutine(durationCoroutine);
            durationCoroutine = null;
        }
        durationCoroutine = StartCoroutine(BannerDuration());
    }

    void End() {
        EndBuffs();
        _amountOfBuffs = 0;
        ReturnObject();
    }

    void EndBuffs() {

        List<GameObject> playersRemoved = new(activePlayers);
        activePlayers.Clear();

        if (_level == 4) {
            for (int i = 0; i < _amountOfBuffs; i++) {
                _damageManager.DecreaseBaseAttack(_info.BaseAttackIncreaseLevel2);
            }
        }

        else if (_level > 1) {
            foreach (var player in playersRemoved) {
                player.GetComponent<DamageManager>().DecreaseBaseAttack(_info.BaseAttackIncreaseLevel2);
            }
            _damageManager.DecreaseBaseAttack(_info.BaseAttackIncreaseLevel2);
        }

        else {
            foreach (var player in playersRemoved) {
                player.GetComponent<DamageManager>().DecreaseBaseAttack(_info.BaseAttackIncrease);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (_level < 2) {
            if (!activePlayers.Contains(other.gameObject)) {
                activePlayers.Add(other.gameObject);
                other.GetComponent<DamageManager>().IncreaseBaseAttack(_info.BaseAttackIncrease);
            }
        }
        else if (_level < 3 && other.CompareTag("Mel")) {
            if (!activePlayers.Contains(other.gameObject)) {
                activePlayers.Add(other.gameObject);
                other.GetComponent<DamageManager>().IncreaseBaseAttack(_info.BaseAttackIncreaseLevel2);
            }
        }
        else {
            return;
        }
    }

    private void OnTriggerExit(Collider other) {

        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (_level < 2) {
            if (!activePlayers.Contains(other.gameObject)) {
                activePlayers.Remove(other.gameObject);
                other.GetComponent<DamageManager>().DecreaseBaseAttack(_info.BaseAttackIncrease);
            }
        }
        else if (_level < 3 && other.CompareTag("Mel")) {
            if (!activePlayers.Contains(other.gameObject)) {
                activePlayers.Remove(other.gameObject);
                other.GetComponent<DamageManager>().DecreaseBaseAttack(_info.BaseAttackIncreaseLevel2);
            }
        }
        else {
            return;
        }
    }

    public override void AddStack() {
        OnStacked?.Invoke(this, EventArgs.Empty);

        AddBuffs();

        if (_info.Character == LocalWhiteBoard.Instance.PlayerCharacter) {
            _maveis.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, _info);
        }
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
