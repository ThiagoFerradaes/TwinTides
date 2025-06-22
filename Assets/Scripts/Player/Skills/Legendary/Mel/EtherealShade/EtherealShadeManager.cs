using System.Collections;
using UnityEngine;

public class EtherealShadeManager : SkillObjectPrefab
{
    EtherealShade _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    Animator anim;
    PlayerController _pController;
    PlayerSkillManager _sManager;

    Coroutine attackRoutine;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as EtherealShade;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
            anim = _mel.GetComponentInChildren<Animator>();
            _pController = _mel.GetComponent<PlayerController>();
            _sManager = _mel.GetComponent<PlayerSkillManager>();
        }

        gameObject.SetActive(true);

        attackRoutine ??= StartCoroutine(AttackRoutine());
    }
    IEnumerator AttackRoutine() {
        _pController.BlockMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(true);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);
        anim.SetTrigger("EtherealShade");

        while (anim.IsInTransition(0)) { yield return null; }

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        while (stateInfo.IsName(_info.AnimationName) == false) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        // Espera a animação terminar
        while (stateInfo.normalizedTime < _info.AnimationPercentToAttack) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        Instantiate();

        while (stateInfo.normalizedTime < 1f && stateInfo.IsName(_info.AnimationName)) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }


        ReturnObject();
    }
    void Instantiate() {

        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Mel) return;

        Vector3 skillPos;

        skillPos.y = GetFloorHeight(_context.Pos);

        PlayerController controller = _mel.GetComponent<PlayerController>();
        Vector3 direction = _context.PlayerRotation * Vector3.forward;
        Vector3 position = _context.Pos + (direction * _info.MaxRangeToPlace);

        if (LocalWhiteBoard.Instance.IsAiming && Vector3.Distance(controller.mousePos, _mel.transform.position) < _info.MaxRangeToPlace) {

            skillPos.x = controller.mousePos.x;
            skillPos.z = controller.mousePos.z;
        }
        else {
            skillPos.x = position.x;
            skillPos.z = position.z;
        }


        SkillContext newContext = new(skillPos, transform.rotation, _context.SkillIdInUI);
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, newContext, _level, 1);

    }

    float GetFloorHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 15f, LayerMask.GetMask("Floor"))) return hit.point.y + 0.1f;
        return position.y;
    }

    public override void ReturnObject() {
        _pController.AllowMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
        attackRoutine = null;
        base.ReturnObject();
    }
}
