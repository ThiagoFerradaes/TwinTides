using System.Collections;
using UnityEngine;

public class EchoBlastManager : SkillObjectPrefab {
    EchoBlast _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    Animator anim;
    PlayerController _pController;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as EchoBlast;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            anim = _maevis.GetComponentInChildren<Animator>();
            _pController = _maevis.GetComponentInChildren<PlayerController>();
        }

        EchoBlastStunExplosion.OnSecondaryExplosion += EchoBlastStunExplosion_OnSecondaryExplosion;

        gameObject.SetActive(true);

        StartCoroutine(AttackRoutine());

        StartCoroutine(Duration());
    }

    IEnumerator AttackRoutine() {
        _pController.BlockMovement();

        anim.SetTrigger("EchoBlast");

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        while (anim.IsInTransition(0)) yield return null;

        while (stateInfo.IsName(_info.AnimationName) == false) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        while (stateInfo.normalizedTime < _info.AnimationPercentToAttack) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 1);
        }

        while (stateInfo.normalizedTime < 1) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        _pController.AllowMovement();
    }
    private void EchoBlastStunExplosion_OnSecondaryExplosion(object sender, EchoBlastStunExplosion.ExplosionPosition e) {
        StartCoroutine(SecondaryExplosion(e.context));
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.ManagerDuration);

        ReturnObject();
    }

    IEnumerator SecondaryExplosion(SkillContext context) {
        for (int i = 0; i < _info.ExplosionAmountLevel3; i++) {
            yield return new WaitForSeconds(_info.TimeBetweenEachExplosion);
            if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
                int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
                PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, context, _level, 3);
            }
        }
    }

    public override void ReturnObject() {
        _pController.AllowMovement();
        base.ReturnObject();
    }
}
