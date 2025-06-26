using System;
using System.Collections;
using UnityEngine;

public class MaevisNormalAttackManager : SkillObjectPrefab {
    MaevisNormalAttack _info;
    SkillContext _context;
    DamageManager _dManager;

    int _currentAttackCombo = 1;
    GameObject _maevis;
    bool _canAttackAgain = false;

    public event EventHandler OnEndOfAttack;
    Animator anim;

    Coroutine currentAttackCoroutine;
    Coroutine currentDurationCoroutine;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as MaevisNormalAttack;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            anim = _maevis.GetComponentInChildren<Animator>();
            _dManager = _maevis.GetComponent<DamageManager>();
        }

        DefineParent();
    }

    private void DefineParent() {

        transform.SetParent(_maevis.transform);

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        gameObject.SetActive(true);

        currentAttackCoroutine = StartCoroutine(_currentAttackCombo < 3 ? AttackCoroutine() : ThirdAttackCoroutine());

    }

    IEnumerator AttackCoroutine() {
        #region Resets De Inicio
        if (currentDurationCoroutine != null) { // Parando o timer entre ataques
            StopCoroutine(currentDurationCoroutine);
            currentDurationCoroutine = null;
        }

        _canAttackAgain = false;
        _maevis.GetComponent<PlayerController>().BlockMovement();
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);
        #endregion

        #region Animation

        float baseAtkSpeed = _dManager.ReturnBaseAttackSpeed();
        float currentAtkSpeed = _dManager.ReturnAttackSpeed();

        float atkSpeedVar = currentAtkSpeed / baseAtkSpeed;

        anim.SetFloat("AttackSpeed", atkSpeedVar);
        anim.SetInteger(_info.AnimationParameterName, _currentAttackCombo);
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        string animaName = _currentAttackCombo == 1 ? _info.AnimationName : _info.AnimationName2;

        while (anim.IsInTransition(0)) yield return null;

        while (stateInfo.IsName(animaName) == false) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
    
        while (stateInfo.normalizedTime < _info.AnimationPercentToAttack) {// Espera a animação chegar a um ponto
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        anim.SetInteger(_info.AnimationParameterName, 0);

        if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _currentAttackCombo, 1);
        }

        while (stateInfo.normalizedTime < 1) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
        #endregion

        #region Resets Do Final
        _maevis.GetComponent<PlayerController>().AllowMovement();
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);

        _currentAttackCombo++;

        OnEndOfAttack?.Invoke(this, EventArgs.Empty);

        _canAttackAgain = true;

        currentDurationCoroutine = StartCoroutine(TimerBetweenEachAttack());
        #endregion
    }

    IEnumerator ThirdAttackCoroutine() {
        #region Resets Do Inicio
        _canAttackAgain = false;

        if (currentDurationCoroutine != null) {
            StopCoroutine(currentDurationCoroutine);
            currentDurationCoroutine = null;
        }

        _maevis.GetComponent<PlayerController>().BlockMovement();
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);
        #endregion

        #region Animation
        float baseAtkSpeed = _dManager.ReturnBaseAttackSpeed();
        float currentAtkSpeed = _dManager.ReturnAttackSpeed();

        float atkSpeedVar = currentAtkSpeed / baseAtkSpeed;

        anim.SetFloat("AttackSpeed", atkSpeedVar);
        anim.SetInteger(_info.AnimationParameterName, 3);

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        while (stateInfo.IsName(_info.AnimationName3) == false) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        // Espera a animação chegar a um ponto
        while (stateInfo.normalizedTime < _info.ThirdAnimationPercentToAttack) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        // Instancia a hitbox do ataque
        if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) { // Instanciando ataque
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _currentAttackCombo, 1);
        }

        anim.SetInteger(_info.AnimationParameterName, 0);

        while (stateInfo.normalizedTime < 1) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
        #endregion

        #region Resets Do Final

        _maevis.GetComponent<PlayerController>().AllowMovement();
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);

        OnEndOfAttack?.Invoke(this, EventArgs.Empty);

        yield return null;

        ReturnObject();
        #endregion
    }

    IEnumerator TimerBetweenEachAttack() {
        yield return new WaitForSeconds(_info.TimeLimitBetweenEachAttack);

        ReturnObject();
    }

    public override void ReturnObject() {
        if (currentAttackCoroutine != null) {
            StopCoroutine(currentAttackCoroutine);
            currentAttackCoroutine = null;
        }

        if (currentDurationCoroutine != null) {
            StopCoroutine(currentDurationCoroutine);
            currentDurationCoroutine = null;
        }

        _maevis.GetComponent<PlayerController>().AllowMovement();
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);

        _currentAttackCombo = 1;

        _canAttackAgain = true;

        float cooldown = _dManager.ReturnDivisionAttackSpeed(_info.Cooldown);

        if (_info.Character == LocalWhiteBoard.Instance.PlayerCharacter) {
            _maevis.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, cooldown);
        }

        base.ReturnObject();
    }

    public override void AddStack() {
        if (!_canAttackAgain) return;

        if (currentAttackCoroutine != null) StopCoroutine(currentAttackCoroutine);

        currentAttackCoroutine = StartCoroutine(_currentAttackCombo < 3 ? AttackCoroutine() : ThirdAttackCoroutine());
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) { }
}
