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

    float _currentTime = 1;

    public event EventHandler OnEndOfAttack;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as MaevisNormalAttack;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            _dManager = _maevis.GetComponent<DamageManager>();
        }

        DefineParent();
    }

    private void DefineParent() {

        transform.SetParent(_maevis.transform);

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        gameObject.SetActive(true);

        StartCoroutine(AttackCoroutine());

        StartCoroutine(Duration());

    }

    IEnumerator AttackCoroutine() {
        _canAttackAgain = false;

        if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _currentAttackCombo, 1);
        }

        float duration = _currentAttackCombo switch {
            1 => _dManager.ReturnDivisionAttackSpeed(_info.DurationOfFirstAttack),
            2 => _dManager.ReturnDivisionAttackSpeed(_info.DurationOfSecondAttack),
            _ => _dManager.ReturnDivisionAttackSpeed(_info.DurationOfThirdAtack),
        };

        _currentTime = duration + _dManager.ReturnDivisionAttackSpeed(_info.CooldownBetweenEachAttack) + _info.TimeLimitBetweenEachAttack;

        _maevis.GetComponent<PlayerController>().BlockMovement();

        yield return new WaitForSeconds(duration);

        _maevis.GetComponent<PlayerController>().AllowMovement();

        _currentAttackCombo++;

        OnEndOfAttack?.Invoke(this, EventArgs.Empty);

        StartCoroutine(CooldownBetweenAttacks());
    }

    IEnumerator ThirdAttackCoroutine() {
        _canAttackAgain = false;

        if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
            
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _currentAttackCombo, 1);
        }

        float duration = _dManager.ReturnDivisionAttackSpeed(_info.DurationOfThirdAtack);

        _currentTime = duration + _dManager.ReturnDivisionAttackSpeed(_info.CooldownBetweenEachAttack) + _info.TimeLimitBetweenEachAttack;

        _maevis.GetComponent<PlayerController>().BlockMovement();
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);

        yield return new WaitForSeconds(duration);

        OnEndOfAttack?.Invoke(this, EventArgs.Empty);

        _maevis.GetComponent<PlayerController>().AllowMovement();
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);

        yield return null;

        ReturnObject();
    }

    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(_dManager.ReturnDivisionAttackSpeed(_info.CooldownBetweenEachAttack));
        _canAttackAgain = true;
    }

    IEnumerator Duration() {
        while (_currentTime > 0) {
            _currentTime -= Time.deltaTime;
            yield return null;
        }

        ReturnObject();
    }

    public override void ReturnObject() {
        _currentAttackCombo = 1;

        _canAttackAgain = true;

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        float cooldown = _dManager.ReturnDivisionAttackSpeed(_info.Cooldown);

        if (_info.Character == LocalWhiteBoard.Instance.PlayerCharacter) {
            _maevis.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, cooldown);
        }

        base.ReturnObject();
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }

    public override void AddStack() {
        if (!_canAttackAgain) return;

        if (_currentAttackCombo < 3) {
            StartCoroutine(AttackCoroutine());
        }
        else {
            StartCoroutine(ThirdAttackCoroutine());
        }
    }
}
