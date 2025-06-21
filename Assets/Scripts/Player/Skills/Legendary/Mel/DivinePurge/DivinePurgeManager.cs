using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using UnityEngine;

public class DivinePurgeManager : SkillObjectPrefab
{
    DivinePurge _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    PlayerController _pController;
    PlayerSkillManager _sManager;
    Animator anim;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as DivinePurge;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
            _pController = _mel.GetComponent<PlayerController>();
            _sManager = _mel.GetComponent<PlayerSkillManager>();
            anim = _mel.GetComponentInChildren<Animator>();
        }

        gameObject.SetActive(true);

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine() {
        _pController.BlockMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(true);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);
        anim.SetBool("DivinePurge", true);

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        while (anim.IsInTransition(0)) yield return null;

        while (stateInfo.IsName(_info.AnimationName) == false) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        // Espera a animação terminar
        while (stateInfo.normalizedTime < _info.AnimationPercentToAttack) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        if (_info.Character == LocalWhiteBoard.Instance.PlayerCharacter) {
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 1);
        }

        while (stateInfo.normalizedTime < 1f && stateInfo.IsName(_info.AnimationName)) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        ReturnObject();
    }

    public override void ReturnObject() {
        anim.SetBool("DivinePurge", false);
        _pController.AllowMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
        base.ReturnObject();
    }
}
