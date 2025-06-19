using System;
using System.Collections;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using UnityEngine;


public class MelNormalAttackManager : SkillObjectPrefab
{
    MelNormalAttack _info;
    SkillContext _context;

    GameObject _mel;
    DamageManager _dManager;
    Animator anim;
    PlayerController _pController;
    PlayerSkillManager _sManager;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as MelNormalAttack;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
            _dManager = _mel.GetComponent<DamageManager>();
            anim = _mel.GetComponentInChildren<Animator>();
            _pController = _mel.GetComponentInChildren<PlayerController>();
            _sManager = _mel.GetComponentInChildren<PlayerSkillManager>();
        }

        gameObject.SetActive(true);

        StartCoroutine(AttackRoutine());

    }

    IEnumerator AttackRoutine() {

        float currentAttackSpeed = _dManager.ReturnAttackSpeed();
        float originalAttackSpeed = _dManager.ReturnBaseAttackSpeed();

        float atkSpeedVar = currentAttackSpeed / originalAttackSpeed;

        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(true);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);


        int attack = UnityEngine.Random.Range(0, 4);
        anim.SetFloat("Ataques", attack);
        anim.SetFloat("AttackSpeed", atkSpeedVar);
        anim.SetTrigger("IsAttacking");

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
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, 0, 1);
        }

        while (stateInfo.normalizedTime < 1f && stateInfo.IsName(_info.AnimationName)) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        ReturnObject();
    }

    public override void ReturnObject() {

        if (_info.Character == LocalWhiteBoard.Instance.PlayerCharacter) {
            float cooldown = _dManager.ReturnDivisionAttackSpeed(_info.Cooldown);
            _mel.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, cooldown);
        }

        StopAllCoroutines();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
        base.ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
