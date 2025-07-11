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
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);
        _maevis.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(true);

        anim.SetTrigger("EchoBlast");

        float enterAnimTimeout = 1f;
        float timer = 0f;

        while (anim.IsInTransition(0)) {
            yield return null;
            timer += Time.deltaTime;
            if (timer > enterAnimTimeout) {
                Debug.LogWarning("Transi��o para anima��o nunca come�ou.");
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
                Debug.LogWarning("Anima��o correta nunca entrou. Cancelando CryRoutine.");
                break;
            }
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
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
        _maevis.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
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
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
        _maevis.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        base.ReturnObject();
    }
}
