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

        if (_maveis == null) {
            _maveis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        InvocateBanner();
    }

    private void InvocateBanner() {
        if (_level < 2) {
            Vector3 direction = _context.PlayerRotation * Vector3.forward;
            Vector3 position = _context.PlayerPosition + (direction * _info.MaxRange);
            Transform aim = _maveis.GetComponent<PlayerController>().aimObject.transform;
            
            if (IsServer) {
                if (aim != null && aim.gameObject.activeInHierarchy && Vector3.Distance(_context.PlayerPosition, aim.position) < _info.MaxRange) {

                    transform.SetPositionAndRotation(aim.position, _context.PlayerRotation);
                }
                else {
                    transform.SetPositionAndRotation(position, _context.PlayerRotation);
                }
            }

            gameObject.SetActive(true);

            durationCoroutine = StartCoroutine(BannerDuration());
        }
        else {

            if (IsServer) {
                transform.SetParent(_maveis.transform);

                transform.SetLocalPositionAndRotation(_info.BannerFollowPosition, Quaternion.Euler(0, 0, 0));

                if (_level > 2) {
                    int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);

                    PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 1);
                }
            }

            gameObject.SetActive(true);

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
        if (!IsServer) return;
        if (_amountOfBuffs < _info.BannerMaxStacks) {
            _amountOfBuffs++;
            _maveis.GetComponent<DamageManager>().IncreaseBaseAttackRpc(_info.BaseAttackIncreaseLevel2);
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
        if (!IsServer) return;

        List<GameObject> playersRemoved = new(activePlayers);
        activePlayers.Clear();

        if (_level == 4) {
            _maveis.GetComponent<DamageManager>().DecreaseBaseAttackRpc(_amountOfBuffs * _info.BaseAttackIncreaseLevel2);
            Debug.Log(_maveis.GetComponent<DamageManager>().ReturnBaseAttack());
        }

        else if (_level > 1) {
            foreach (var player in playersRemoved) {
                player.GetComponent<DamageManager>().DecreaseBaseAttackRpc(_info.BaseAttackIncreaseLevel2);
            }
            _maveis.GetComponent<DamageManager>().DecreaseBaseAttackRpc(_info.BaseAttackIncreaseLevel2);
        }

        else {
            foreach (var player in playersRemoved) {
                player.GetComponent<DamageManager>().DecreaseBaseAttackRpc(_info.BaseAttackIncrease);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (_level < 2) {
            if (!activePlayers.Contains(other.gameObject)) {
                activePlayers.Add(other.gameObject);
                other.GetComponent<DamageManager>().IncreaseBaseAttackRpc(_info.BaseAttackIncrease);
            }
        }
        else if (_level < 3 && other.CompareTag("Mel")) {
            if (!activePlayers.Contains(other.gameObject)) {
                activePlayers.Add(other.gameObject);
                other.GetComponent<DamageManager>().IncreaseBaseAttackRpc(_info.BaseAttackIncreaseLevel2);
            }
        }
        else {
            return;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!IsServer) return;

        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (_level < 2) {
            if (!activePlayers.Contains(other.gameObject)) {
                activePlayers.Remove(other.gameObject);
                Debug.Log("Buff Down in both");
                other.GetComponent<DamageManager>().DecreaseBaseAttackRpc(_info.BaseAttackIncrease);
            }
        }
        else if (_level < 3 && other.CompareTag("Mel")) {
            if (!activePlayers.Contains(other.gameObject)) {
                activePlayers.Remove(other.gameObject);
                other.GetComponent<DamageManager>().DecreaseBaseAttackRpc(_info.BaseAttackIncreaseLevel2);
            }
        }
        else {
            return;
        }
    }


    [Rpc(SendTo.ClientsAndHost)]
    public override void AddStackRpc() {
        OnStacked?.Invoke(this, EventArgs.Empty);
        AddBuffs();
        _maveis.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, _info);
    }
}
