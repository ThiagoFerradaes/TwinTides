using FMODUnity;
using System.Collections;
using UnityEngine;

public class CrimsonTideManager : SkillObjectPrefab {

    CrimsonTide _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    PlayerSkillManager _skillManager;
    PlayerController _playerController;
    Animator anim;
    HealthManager _hManager;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as CrimsonTide;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            _skillManager = _maevis.GetComponent<PlayerSkillManager>();
            anim = _maevis.GetComponentInChildren<Animator>();
            _playerController = _maevis.GetComponent<PlayerController>();
            _hManager = _maevis.GetComponent<HealthManager>();
        }

        DefineParent();
    }

    private void DefineParent() {
        transform.SetParent(_maevis.transform);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        gameObject.SetActive(true);

        _skillManager.BlockNormalAttackRpc(true);
        _skillManager.BlockSkillsRpc(true);

        if (_level < 2) {
            StartCoroutine(PunchRoutine());
        }
        else {
            StartCoroutine(Dash());

            if (_level == 4) StartCoroutine(SpawnPath());
        }
    }
    IEnumerator PunchRoutine() {

        anim.SetTrigger("CrimsonTide");

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        while (anim.IsInTransition(0)) yield return null;

        _playerController.BlockMovement();

        while (stateInfo.IsName(_info.PunchAnimationName) == false) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        while (stateInfo.normalizedTime < _info.PunchAnimationPercentToAttack) { // Espera a animação terminar
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

        _playerController.AllowMovement();

        ReturnObject();

    }
    IEnumerator Dash() {
        float elapsedTime = 0f;

        _playerController.BlockMovement();

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);

        anim.SetBool("CrimsonTideDash", true);
        _hManager.InvulnerabilityRpc(true);

        if (!_info.DashSound.IsNull) RuntimeManager.PlayOneShot(_info.DashSound, transform.position);

        if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 2);
        }

        while (elapsedTime < _info.DashDuration) {
            elapsedTime += Time.deltaTime;
            _maevis.transform.position += _info.DashSpeed * Time.deltaTime * _maevis.transform.forward;
            yield return null;
        }

        if (_level > 2) {
            if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
                SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);
                PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, newContext, _level, 3);
            }
        }


        ReturnObject();
    }

    IEnumerator SpawnPath() {
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        Vector3 lastSpawnPosition = _maevis.transform.position;
        while (true && LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
            if (Vector3.Distance(lastSpawnPosition, _maevis.transform.position) >= _info.PathSpawnInterval) {
                SkillContext newContext = new(_maevis.transform.position, transform.rotation, _context.SkillIdInUI);
                PlayerSkillPooling.Instance.RequestInstantiateNoChecksRpc(skillId, newContext, _level, 4);
                lastSpawnPosition = _maevis.transform.position;
            }
            yield return null;
        }
    }

    public override void ReturnObject() {
        _maevis.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
        _maevis.GetComponent<PlayerController>().AllowMovement();
        anim.SetBool("CrimsonTideDash", false);
        _hManager.InvulnerabilityRpc(false);
        base.ReturnObject();
    }
}
