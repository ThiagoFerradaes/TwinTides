using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MaevisNormalAttackManager : SkillObjectPrefab {
    MaevisNormalAttack _info;
    SkillContext _context;

    int _currentAttackCombo = 1;
    GameObject _maevis;
    bool _canAttackAgain = false;

    float _currentTime = 1;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as MaevisNormalAttack;
        _context = context;

        DefineParent();
    }

    private void DefineParent() {
        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        transform.SetParent(_maevis.transform);

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        gameObject.SetActive(true);

        StartCoroutine(AttackCoroutine());

        StartCoroutine(Duration());

    }

    IEnumerator AttackCoroutine() {
        _canAttackAgain = false;
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _currentAttackCombo, 1);

        float startAngle = transform.localEulerAngles.y;
        float targetAngle = _currentAttackCombo == 1 ? startAngle + 180 : startAngle - 180;

        float elapsedTime = 0f;
        float duration = _currentAttackCombo switch {
            1 => _info.DurationOfFirstAttack,
            2 => _info.DurationOfSecondAttack,
            _ => _info.DurationOfThirdAtack,
        };

        _currentTime = duration + _info.TimeBetweenEachAttack + _info.WindowBetweenEachAttack;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float yRotation = Mathf.Lerp(startAngle, targetAngle, t);
            transform.localRotation = Quaternion.Euler(0, yRotation, 0);
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0, targetAngle, 0);
        _currentAttackCombo++;

        StartCoroutine(CooldownBetweenAttacks());
    }

    IEnumerator ThirdAttackCoroutine() {
        _canAttackAgain = false;
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _currentAttackCombo, 1);

        float startAngle = transform.localEulerAngles.x;
        float targetAngle = startAngle + 90;

        float elapsedTime = 0f;
        float duration = _info.DurationOfThirdAtack;

        _currentTime = duration + _info.TimeBetweenEachAttack + _info.WindowBetweenEachAttack;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float xRotation = Mathf.Lerp(startAngle, targetAngle, t);
            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0, targetAngle, 0);

        yield return null;

        End();
    }

    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(_info.TimeBetweenEachAttack);
        _canAttackAgain = true;
    }

    IEnumerator Duration() {
        while (_currentTime > 0) {
            _currentTime -= Time.deltaTime;
            yield return null;
        }

        End();
    }

    void End() {
        _currentAttackCombo = 1;
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));
        _maevis.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, _info);

        ReturnObject();
    }
    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public override void AddStackRpc() {
        if (!_canAttackAgain) return;

        if (_currentAttackCombo < 3) {
            StartCoroutine(AttackCoroutine());
        }
        else {
            StartCoroutine(ThirdAttackCoroutine());
        }
    }
}
