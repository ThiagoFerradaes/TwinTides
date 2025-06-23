using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class WarCryManager : SkillObjectPrefab {
    Warcry _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    DamageManager _dManager;
    MovementManager _mManager;
    Animator anim;
    PlayerController _pController;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Warcry;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            _dManager = _maevis.GetComponent<DamageManager>();
            anim = _maevis.GetComponentInChildren<Animator>();
            _mManager = _maevis.GetComponent<MovementManager>();
            _pController = _maevis.GetComponent<PlayerController>();
        }

        DefineParentAndPosition();
    }

    private void DefineParentAndPosition() {

        transform.SetParent(_maevis.transform);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        gameObject.SetActive(true);

        StartCoroutine(CryRoutine());

        StartCoroutine(Duration());
    }
    IEnumerator CryRoutine() {
        _pController.BlockMovement();
        _maevis.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(true);
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);

        anim.SetTrigger("WarCry");

        float enterAnimTimeout = 1f;
        float timer = 0f;

        while (anim.IsInTransition(0)) {
            yield return null;
            timer += Time.deltaTime;
            if (timer > enterAnimTimeout) {
                Debug.LogWarning("Transição para animação nunca começou.");
                break;
            }
        }

        timer = 0f;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName(_info.AnimationName)) {
            yield return null;
            timer += Time.deltaTime;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (timer > enterAnimTimeout) {
                Debug.LogWarning("Animação correta nunca entrou. Cancelando CryRoutine.");
                break;
            }
        }

        // Espera a animação terminar
        while (stateInfo.normalizedTime < _info.AnimationPercentToAttack) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        Explode();

        while (stateInfo.normalizedTime < 1) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        _pController.AllowMovement();
        _maevis.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);

    }
    private void Explode() {
        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Maevis) return;

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        SkillContext newContex = new(transform.position, transform.rotation, _context.SkillIdInUI);
        PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, newContex, _level, 1);
    }

    IEnumerator Duration() {
        ApplyBuffs();

        float duration = _level < 4 ? _info.Duration : _info.DurationLevel4;

        yield return new WaitForSeconds(duration);

        ReturnObject();
    }

    void ApplyBuffs() {

        if (_level > 3) {
            LocalWhiteBoard.Instance.PlayerAttackSkill = _info.EnhancedMaevisAttack;
            _dManager.IncreaseAttackSpeed(_info.PercentAttackSpeedLevel4);
        }
        else {
            float percent = _level < 2 ? _info.PercentAttackSpeed : _info.PercentAttackSpeedLevel2;
            _dManager.IncreaseAttackSpeed(percent);
        }

        if (_level > 2) {
            _mManager.IncreaseMoveSpeed(_info.PercentMoveSpeedGain);
        }
    }
    void RemoveBuffs() {

        if (_level > 3) {
            LocalWhiteBoard.Instance.PlayerAttackSkill = _info.NormalMaevisAttack;
            _dManager.DecreaseAttackSpeed(_info.PercentAttackSpeedLevel4);
        }
        else {
            float percent = _level < 2 ? _info.PercentAttackSpeed : _info.PercentAttackSpeedLevel2;
            _dManager.DecreaseAttackSpeed(percent);
        }

        if (_level > 2) {
            _mManager.DecreaseMoveSpeed(_info.PercentMoveSpeedGain);
        }

    }

    public override void ReturnObject() {
        RemoveBuffs();
        _pController.AllowMovement();
        _maevis.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
        base.ReturnObject();
    }
}
