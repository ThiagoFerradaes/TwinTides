using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillManager : NetworkBehaviour {

    #region Events
    public event EventHandler<SkillEventHandler> OnBaseAttack;
    public event EventHandler<SkillEventHandler> OnCommonSkillOne;
    public event EventHandler<SkillEventHandler> OnCommonSkillTwo;
    public event EventHandler<SkillEventHandler> OnLegendary;

    public class SkillEventHandler : EventArgs {
        public float SkillCooldown;
        public SkillType Type;

        public SkillEventHandler(SkillType type ,float skillCooldown) {
            SkillCooldown = skillCooldown;
            Type = type;
        }
    }
    #endregion

    Dictionary<int, float> _dictionaryOfCooldowns;
    NetworkVariable<bool> _canNormalAttack = new(true);
    NetworkVariable<bool> _canUseSkill = new(true);

    MovementManager _mManager;

    private void Start() {
        _dictionaryOfCooldowns = new Dictionary<int, float> {
            { 0, 0f },
            { 1, 0f },
            { 2, 0f },
            { 3, 0f },
        };

        _mManager = GetComponent<MovementManager>();

        PlayersDeathManager.OnGameRestart += RestartAllCooldowns;
    }

    private void RestartAllCooldowns() {
        for (int i = 0; i < _dictionaryOfCooldowns.Count; i++) {
            ResetCooldown(i);
        }
    }

    #region Inputs
    public void InputBaseAttack(InputAction.CallbackContext context) {
        if (LocalWhiteBoard.Instance.AnimationOn) return;

        if (context.phase == InputActionPhase.Performed && _dictionaryOfCooldowns[0] <= 0 && _canNormalAttack.Value) {
            UseSkill(0);
        }
    }
    public void InputCommonRelicSkillOne(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started && _dictionaryOfCooldowns[1] <= 0 && _canUseSkill.Value) {
            UseSkill(1);
        }
        
    }
    public void InputCommonRelicSkillTwo(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && _dictionaryOfCooldowns[2] <= 0 && _canUseSkill.Value) {
            UseSkill(2);
        }    
    }
    public void InputLegendarySkill(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && _dictionaryOfCooldowns[3] <= 0 && _canUseSkill.Value) {
            UseSkill(3);
        }  
    }
    #endregion

    void UseSkill(int skillId) {
        if (_mManager.ReturnStunnedValue()) return;

        SkillContext skillContext = new(this.transform.position, this.transform.rotation, skillId);
        switch (skillId) {
            case 0:
                if (LocalWhiteBoard.Instance.PlayerAttackSkill != null) {
                    AttackSkill skill = LocalWhiteBoard.Instance.PlayerAttackSkill;
                    skill.UseSkill(skillContext, LocalWhiteBoard.Instance.AttackLevel);
                }
                else Debug.Log("No Skill");
                break;
            case 1:
                if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne != null) {
                    CommonRelic skill = LocalWhiteBoard.Instance.PlayerCommonRelicSkillOne;
                    skill.UseSkill(skillContext, LocalWhiteBoard.Instance.CommonRelicInventory[skill]);
                }      
                else Debug.Log("No Skill");
                break;
            case 2:
                if (LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo != null) {
                    CommonRelic skill = LocalWhiteBoard.Instance.PlayerCommonRelicSkillTwo;
                    skill.UseSkill(skillContext, LocalWhiteBoard.Instance.CommonRelicInventory[skill]);
                }
                else Debug.Log("No Skill");
                break;
            case 3:
                if (LocalWhiteBoard.Instance.PlayerLegendarySkill != null) {
                    LegendaryRelic skill = LocalWhiteBoard.Instance.PlayerLegendarySkill;
                    skill.UseSkill(skillContext, LocalWhiteBoard.Instance.LegendaryRelicInventory[skill]);
                }
                else Debug.Log("No Skill");
                break;
        }
    }
    public void StartCooldown(int skillIdUI, Skill skill) {
        switch (skillIdUI) {
            case 0:
                StartCoroutine(SetCooldown(0, skill.Cooldown));
                OnBaseAttack?.Invoke(this, new SkillEventHandler(SkillType.Attack, skill.ReturnCooldown()));
                break;
            case 1:
                StartCoroutine(SetCooldown(1, skill.Cooldown));
                OnCommonSkillOne?.Invoke(this, new SkillEventHandler(SkillType.CommonRelicOne, skill.ReturnCooldown()));
                break;
            case 2:
                StartCoroutine(SetCooldown(2, skill.Cooldown));
                OnCommonSkillTwo?.Invoke(this, new SkillEventHandler(SkillType.CommonRelicTwo, skill.ReturnCooldown()));
                break;
            case 3:
                StartCoroutine(SetCooldown(3, skill.Cooldown));
                OnLegendary?.Invoke(this, new SkillEventHandler(SkillType.LegendaryRelic, skill.ReturnCooldown()));
                break;
        }
    }

    public void StartCooldown(int skillIdUI, float cooldown) {
        switch (skillIdUI) {
            case 0:
                StartCoroutine(SetCooldown(0, cooldown));
                OnBaseAttack?.Invoke(this, new SkillEventHandler(SkillType.Attack, cooldown));
                break;
            case 1:
                StartCoroutine(SetCooldown(1, cooldown));
                OnCommonSkillOne?.Invoke(this, new SkillEventHandler(SkillType.CommonRelicOne, cooldown));
                break;
            case 2:
                StartCoroutine(SetCooldown(2, cooldown));
                OnCommonSkillTwo?.Invoke(this, new SkillEventHandler(SkillType.CommonRelicTwo, cooldown));
                break;
            case 3:
                StartCoroutine(SetCooldown(3, cooldown));
                OnLegendary?.Invoke(this, new SkillEventHandler(SkillType.LegendaryRelic, cooldown));
                break;
        }
    }

    IEnumerator SetCooldown(int skillId,float cooldown) {
        _dictionaryOfCooldowns[skillId] = cooldown;
        while (_dictionaryOfCooldowns[skillId] >= 0) {
            _dictionaryOfCooldowns[skillId] -= Time.deltaTime;
            yield return null;
        }
    }

    public void ResetCooldown(int skillId) {
        _dictionaryOfCooldowns[skillId] = 0;
        switch (skillId) {
            case 0:
                OnBaseAttack?.Invoke(this, new SkillEventHandler(SkillType.Attack, 0));
                break;
            case 1:
                OnCommonSkillOne?.Invoke(this, new SkillEventHandler(SkillType.CommonRelicOne, 0));
                break;
            case 2:
                OnCommonSkillTwo?.Invoke(this, new SkillEventHandler(SkillType.CommonRelicTwo, 0));
                break;
            case 3:
                OnLegendary?.Invoke(this, new SkillEventHandler(SkillType.LegendaryRelic, 0));
                break;
        }
    }

    /// <summary>
    /// Função para bloquear o ataque normal do personagem (se passar verdadeiro ele bloqueia, se passar falso ele desbloqueia)
    /// </summary>
    /// <param name="block"></param>
    [Rpc(SendTo.Server)]
    public void BlockNormalAttackRpc(bool block) => _canNormalAttack.Value = !block;

    /// <summary>
    /// Função para bloquear o uso de skills do personagem (se passar verdadeiro ele bloqueia, se passar falso ele desbloqueia)
    /// </summary>
    /// <param name="block"></param>
    [Rpc(SendTo.Server)]
    public void BlockSkillsRpc(bool block) => _canUseSkill.Value = !block;
}
