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
    int _amountOfBuffs;

    List<GameObject> activePlayers = new();
    Coroutine durationCoroutine;
    public static event EventHandler OnStacked;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as FallenBanner;
        _level = skillLevel;
        _context = context;

        DefineParent();
    }

    private void DefineParent() {
        if (_maveis == null) {
            _maveis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        InvocateBanner();
    }

    private void InvocateBanner() {
        if (_level < 2) {
            Vector3 direction = _context.PlayerRotation * Vector3.forward;
            Vector3 position = _context.PlayerPosition + (direction * _info.Offset);
            transform.SetPositionAndRotation(position, _context.PlayerRotation);

            gameObject.SetActive(true);

            durationCoroutine = StartCoroutine(BannerDuration());
        }
        else {

            transform.SetParent(_maveis.transform);

            transform.SetLocalPositionAndRotation(_info.BannerFollowPosition, Quaternion.Euler(0, 0, 0));

            gameObject.SetActive(true);

            if (_level > 2) {
                int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);

                PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 1);
            }

            AddBuffs();
        }
    }

    IEnumerator BannerDuration() {
        float duration;
        if (_level < 4) duration = _info.BannerDuration;
        else duration = _info.BannerDurationLevel4;

        yield return new WaitForSeconds(duration);

        End();
    }

    void AddBuffs() {
        if (_amountOfBuffs >= _info.BannerMaxStacks) {

            Debug.Log("MaxBuffs!");
        }
        else {
            _amountOfBuffs++;
            Debug.Log("BuffUP: " + _amountOfBuffs);
        }

        if (durationCoroutine != null) StopCoroutine(durationCoroutine);
        durationCoroutine = StartCoroutine(BannerDuration());
    }

    void End() {
        _amountOfBuffs = 0;
        foreach (var player in activePlayers) Debug.Log("Buff Down");
        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (_level < 2) {
            activePlayers.Add(other.gameObject);
            Debug.Log("Buff Up in both");
        }
        else if (_level < 3 && other.CompareTag("Mel")) {
            activePlayers.Add(other.gameObject);
            Debug.Log("Buff Up in Mel");
        }
        else {
            return;
        }
    }

        private void OnTriggerExit(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (_level < 2) {
            activePlayers.Remove(other.gameObject);
            Debug.Log("Buff Down in both");
        }
        else if (_level < 3 && other.CompareTag("Mel")) {
            activePlayers.Remove(other.gameObject);
            Debug.Log("Buff Down in Mel");
        }
        else {
            return;
        }
    }


    [Rpc(SendTo.ClientsAndHost)]
    public override void AddStackRpc() {
        OnStacked?.Invoke(this, EventArgs.Empty);
        AddBuffs();
    }
}
