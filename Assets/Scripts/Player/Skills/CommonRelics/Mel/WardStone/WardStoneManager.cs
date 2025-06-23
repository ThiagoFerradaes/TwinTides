using System.Collections;
using UnityEngine;

public class WardStoneManager : SkillObjectPrefab
{
    WardStone _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    PlayerController _pController;
    PlayerSkillManager _sManager;
    Animator anim;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as WardStone;
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
        anim.SetTrigger("WardStone");

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
        _pController.AllowMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
        base.ReturnObject();
    }
}
