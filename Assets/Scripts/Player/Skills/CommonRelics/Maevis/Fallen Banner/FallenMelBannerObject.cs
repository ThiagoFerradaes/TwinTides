using System;
using System.Collections;
using UnityEngine;

public class FallenMelBannerObject : SkillObjectPrefab {
    FallenBanner _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    DamageManager _damageManager;
    int _amountOfBuffs;

    Coroutine durationCoroutine;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as FallenBanner;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
            _damageManager = _mel.GetComponent<DamageManager>();
        }


        FallenMaevisBannerObject.OnStacked += FallenMaevisBannerObject_OnStacked;

        InvocateBanner();

    }

    private void FallenMaevisBannerObject_OnStacked(object sender, EventArgs e) {
        AddBuffs();
    }

    private void InvocateBanner() {
        transform.SetParent(_mel.transform);

        transform.SetLocalPositionAndRotation(_info.BannerFollowPosition, Quaternion.Euler(0, 0, 0));


        gameObject.SetActive(true);

        durationCoroutine = StartCoroutine(BannerDuration());

        AddBuffs();
    }

    IEnumerator BannerDuration() {
        float duration = _level < 4 ? _info.BannerDuration : _info.BannerDurationLevel4;

        yield return new WaitForSeconds(duration);

        End();
    }

    private void End() {
        EndBuffs();
        _amountOfBuffs = 0;
        ReturnObject();
    }
    void EndBuffs() {

        for (int i = 0; i < _amountOfBuffs; i++) {
            _damageManager.DecreaseBaseAttackRpc(_info.BaseAttackIncreaseLevel2);
        }

    }
    void AddBuffs() {

        if (_amountOfBuffs < _info.BannerMaxStacks) {
            _amountOfBuffs++;
            _damageManager.IncreaseBaseAttackRpc(_info.BaseAttackIncreaseLevel2);
        }

        if (durationCoroutine != null) StopCoroutine(durationCoroutine);
        durationCoroutine = StartCoroutine(BannerDuration());
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}

