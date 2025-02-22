using System;
using System.Collections;
using UnityEngine;

public class FallenMelBannerObject : SkillObjectPrefab {
    FallenBanner _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    int _amountOfBuffs;

    Coroutine durationCoroutine;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as FallenBanner;
        _level = skillLevel;
        _context = context;

        DefineParent();
    }

    private void DefineParent() {
        if (_mel == null) {
            if (PlayerSkillPooling.Instance.MelGameObject != null) {
                _mel = PlayerSkillPooling.Instance.MelGameObject;
            }
            else {
                End();
                return;
            }
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
        float duration;
        if (_level < 4) duration = _info.BannerDuration;
        else duration = _info.BannerDurationLevel4;

        yield return new WaitForSeconds(duration);

        End();
    }

    private void End() {
        EndBuffs();
        _amountOfBuffs = 0;
        ReturnObject();
    }
    void EndBuffs() {
        if (!IsServer) return;
        if (_level == 4) {
            _mel.GetComponent<DamageManager>().DecreaseBaseAttackRpc(_amountOfBuffs * _info.BaseAttackIncreaseLevel2);
            Debug.Log(_mel.GetComponent<DamageManager>().ReturnBaseAttack());
        }
        else {
            _mel.GetComponent<DamageManager>().DecreaseBaseAttackRpc(_info.BaseAttackIncreaseLevel2);
            Debug.Log(_mel.GetComponent<DamageManager>().ReturnBaseAttack());
        }
    }
    void AddBuffs() {
        if (_level < 4) {
            _mel.GetComponent<DamageManager>().IncreaseBaseAttackRpc(_info.BaseAttackIncreaseLevel2);
        }
        else {
            if (_amountOfBuffs >= _info.BannerMaxStacks) {
                Debug.Log("MaxBuffs!");
            }
            else {
                _amountOfBuffs++;
                _mel.GetComponent<DamageManager>().IncreaseBaseAttackRpc(_info.BaseAttackIncreaseLevel2);
                Debug.Log("BuffUP: " + _amountOfBuffs);
            }
        }

        if (durationCoroutine != null) StopCoroutine(durationCoroutine);
        durationCoroutine = StartCoroutine(BannerDuration());
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}

