using System.Collections;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using Unity.Netcode;
using UnityEngine;

public class GhostlyWhisperManager : SkillObjectPrefab {
    GhostlyWhispers _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    int amountOfPuddles;
    Animator anim;
    PlayerController _pController;
    PlayerSkillManager _sManager;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as GhostlyWhispers;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
            anim = _mel.GetComponentInChildren<Animator>();
            _pController = _mel.GetComponentInChildren<PlayerController>();
            _sManager = _mel.GetComponentInChildren<PlayerSkillManager>();
        }

        DefinePosition();
    }

    void DefinePosition() {

        transform.SetParent(_mel.transform);

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        DefineAmountOfPuddles();

        gameObject.SetActive(true);

        StartCoroutine(AttackRoutine());

        if (_level > 1) { Debug.Log("Skill level > 1"); StartCoroutine(Duration()); }
    }

    IEnumerator AttackRoutine() {
        _pController.BlockMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(true);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);
        anim.SetTrigger("GhostlyWhisper");

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        while (anim.IsInTransition(0)) yield return null;

        while (stateInfo.IsName(_info.AnimationName) == false) {
            Debug.Log("Esperando entrar");
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        // Espera a animação terminar
        while (stateInfo.normalizedTime < _info.AnimationPercentToAttack) {
            Debug.Log("Esperando chegar");
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        InstantiatePuddle();

        while (stateInfo.normalizedTime < 1f && stateInfo.IsName(_info.AnimationName)) {
            Debug.Log("Esperando terminar");
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        _pController.AllowMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);

        if (_level == 1) ReturnObject();
    }

    void InstantiatePuddle() {
        Debug.Log("Instanciando");

        amountOfPuddles--;

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
        PlayerSkillPooling.Instance.RequestInstantiateNoChecksRpc(skillId, newContext, _level, 1);
    }

    float GetFloorHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 15f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) return hit.point.y + 0.1f;
        return position.y;
    }

    void DefineAmountOfPuddles() {
        if (_level < 2) {
            amountOfPuddles = _info.AmountOfStacks;
        }
        else if (_level < 4) {
            amountOfPuddles = _info.AmountOfStacksLevel2;
        }
        else {
            amountOfPuddles = _info.AmountOfStacksLevel4;
        }
    }

    IEnumerator Duration() {
        if (_level > 1) {
            float elapsedTime = 0f;
            float duration;
            if (_level < 4) duration = _info.ObjectDurationLevel2;
            else duration = _info.ObjectDurationLevel4;

            while (amountOfPuddles > 0 && elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        yield return null;

        ReturnObject();
    }

    public override void ReturnObject() {
        _pController.AllowMovement();
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
        amountOfPuddles = 0;
        if (_info.Character == LocalWhiteBoard.Instance.PlayerCharacter) {
            _mel.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, _info);
        }
        base.ReturnObject();
    }

    public override void AddStack() {
        StartCoroutine(AttackRoutine());
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
