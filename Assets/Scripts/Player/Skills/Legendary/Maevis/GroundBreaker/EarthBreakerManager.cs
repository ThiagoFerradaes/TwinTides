using System.Collections;
using UnityEngine;

public class EarthBreakerManager : SkillObjectPrefab {
    EarthBreaker _info;
    SkillContext _context;
    GameObject _maevis;
    [HideInInspector] public int _amountOfImpactsSummoned;
    Animator anim;
    PlayerController _pController;
    PlayerSkillManager _sManager;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as EarthBreaker;
        _context = context;
        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            anim = _maevis.GetComponentInChildren<Animator>();
            _pController = _maevis.GetComponentInChildren<PlayerController>();
            _sManager = _maevis.GetComponentInChildren<PlayerSkillManager>();
        }

        SetPosition();
    }

    void SetPosition() {
        _amountOfImpactsSummoned = 0;

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine() {
        _pController.BlockMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(true);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);
        anim.SetTrigger("EarthShatter");

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

        StartCoroutine(Duration());

        while (stateInfo.normalizedTime < 1) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        _pController.AllowMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
    }

    IEnumerator Duration() {
        for (int i = 0; i < _info.AmountOfImpacts; i++) {
            SummonImpact();
            yield return new WaitForSeconds(_info.CooldownBetweenEachImpact);
        }

        ReturnObject();
    }
    void SummonImpact() {

        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Maevis) return;

        _amountOfImpactsSummoned++;

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);

        Vector3 direction = _context.PlayerRotation * Vector3.forward;
        Vector3 newPos = (_context.Pos) + (direction * _info.InicialImpactSize.z * _amountOfImpactsSummoned);
        SkillContext newContext = new(newPos, _context.PlayerRotation, _context.SkillIdInUI);

        PlayerSkillPooling.Instance.RequestInstantiateNoChecksRpc(skillId, newContext, 1, 1);
    }


    public override void ReturnObject() {
        _pController.AllowMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
        base.ReturnObject();
    }

}
