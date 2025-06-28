using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CrimsonTideManager : SkillObjectPrefab {

    CrimsonTide _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    PlayerSkillManager _skillManager;
    PlayerController _playerController;
    Animator anim;
    HealthManager _hManager;
    Rigidbody _rb;

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
            _rb = _maevis.GetComponent<Rigidbody>();
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
        _maevis.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(true);
        _maevis.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);
        _maevis.GetComponent<PlayerController>().BlockMovement();
        anim.SetTrigger("CrimsonTide");

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
        while (!stateInfo.IsName(_info.PunchAnimationName)) {
            yield return null;
            timer += Time.deltaTime;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (timer > enterAnimTimeout) {
                Debug.LogWarning("Anima��o correta nunca entrou. Cancelando CryRoutine.");
                break;
            }
        }

        while (stateInfo.normalizedTime < _info.PunchAnimationPercentToAttack) { // Espera a anima��o terminar
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
        _playerController.InDash(true);

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);

        anim.SetBool("CrimsonTideDash", true);
        _hManager.InvulnerabilityRpc(true);

        if (!_info.DashSound.IsNull) RuntimeManager.PlayOneShot(_info.DashSound, transform.position);

        if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 2);
        }

        _rb.linearVelocity = _maevis.transform.forward * _info.DashSpeed;

        while (elapsedTime < _info.DashDuration) {
            _rb.linearVelocity = _maevis.transform.forward * _info.DashSpeed;
            elapsedTime += Time.deltaTime;
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
        _playerController.InDash(false);
        anim.SetBool("CrimsonTideDash", false);
        _hManager.InvulnerabilityRpc(false);
        base.ReturnObject();
    }
}
