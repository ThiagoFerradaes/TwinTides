using System.Collections;
using UnityEngine;

public class BlackHoleManager : SkillObjectPrefab
{
    BlackHole _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    Animator anim;
    PlayerController _pController;
    PlayerSkillManager _sManager;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as BlackHole;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
            anim = _mel.GetComponentInChildren<Animator>();
            _pController = _mel.GetComponentInChildren<PlayerController>();
            _sManager = _mel.GetComponentInChildren<PlayerSkillManager>();
        }

        gameObject.SetActive(true);

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine() {
        _pController.BlockMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(true);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);
        anim.SetTrigger("BlackHole");

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

        InstantiateBlackHole();

        while (stateInfo.normalizedTime < 1f && stateInfo.IsName(_info.AnimationName)) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        ReturnObject();
    }
    void InstantiateBlackHole() {

        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Mel) return;

        Vector3 skillPos;

        skillPos.y = GetFloorHeight(_context.Pos);

        PlayerController controller = _mel.GetComponent<PlayerController>();
        Vector3 direction = _context.PlayerRotation * Vector3.forward;
        Vector3 position = _context.Pos + (direction * _info.MaxRange);

        if (LocalWhiteBoard.Instance.IsAiming && Vector3.Distance(controller.mousePos, _mel.transform.position) < _info.MaxRange) {

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
        Ray ray = new(position + Vector3.up * 15f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) return hit.point.y + 0.1f;
        return position.y;
    }

    public override void ReturnObject() {
        _pController.AllowMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
        base.ReturnObject();
    }

}
