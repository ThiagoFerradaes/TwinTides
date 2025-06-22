using FMODUnity;
using System.Collections;
using System.Threading;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using UnityEngine;

public class SpiritConvergenceManager : SkillObjectPrefab {
    SpiritConvergence _info;
    int _level, _timesExtended;
    SkillContext _context;
    GameObject _mel;
    HealthManager _hManager;
    float _timer, _durationTime;
    bool _canInstantiateRangedMinion = true;
    Animator anim;
    PlayerSkillManager _sManager;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as SpiritConvergence;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
            _hManager = _mel.GetComponent<HealthManager>();
            _sManager = _mel.GetComponent<PlayerSkillManager>();
            anim = _mel.GetComponentInChildren<Animator>();
        }

        HealthManager.OnMelHealed += OnMelHealed;

        SetParent();
    }

    void SetParent() {

        transform.parent = _mel.transform;

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        gameObject.SetActive(true);

        StartCoroutine(AttackRoutine());

    }
    IEnumerator AttackRoutine() {
        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(true);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(true);
        anim.SetTrigger("SpiritConvergence");

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

        StartCoroutine(WaitAFrame(SkillDuration()));

        while (stateInfo.normalizedTime < 1f && stateInfo.IsName(_info.AnimationName)) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);
    }
    IEnumerator WaitAFrame(IEnumerator corroutine) {
        yield return null;

        StartCoroutine(corroutine);
    }

    IEnumerator SkillDuration() {
        _timer = 0;
        _durationTime = _info.SkillDuration;
        StartCoroutine(InstantiateMeleeMinion());

        while (_timer < _durationTime) {
            _timer += Time.deltaTime;
            yield return null;
        }

        ReturnObject();
    }

    IEnumerator InstantiateMeleeMinion() {
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);

        while (_timer < _durationTime) {

            if(!_info.InvocationSound.IsNull) RuntimeManager.PlayOneShot(_info.InvocationSound, transform.position);

            if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Mel) {
                SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);
                PlayerSkillPooling.Instance.RequestInstantiateNoChecksRpc(skillId, newContext, _level, 1);
            }

            yield return new WaitForSeconds(_info.MeleeMinionCooldown);
        }
    }

    private void OnMelHealed(object sender, System.EventArgs e) {
        if (_canInstantiateRangedMinion) {

            _canInstantiateRangedMinion = false;

            if (_timesExtended < _info.AmountOfTimesDurationCanExtend) {
                _durationTime += _info.DurationExtensionTime;
                _timesExtended++;
            }

            StartCoroutine(RangedMinionCooldown());

            InstantiateRangedMinion();
        }
    }

    IEnumerator RangedMinionCooldown() {
        yield return new WaitForSeconds(_info.RangedMinionCooldown);
        _canInstantiateRangedMinion = true;
    }

    void InstantiateRangedMinion() {
        Debug.Log("Ranged Minion Funcion Called");
        if (!_info.InvocationSound.IsNull) RuntimeManager.PlayOneShot(_info.InvocationSound, transform.position);

        if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Mel) {
            Debug.Log("Ranged Minion Instance");
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);
            PlayerSkillPooling.Instance.RequestInstantiateNoChecksRpc(skillId, newContext, _level, 2);
        }
    }

    public override void ReturnObject() {

        _sManager.GetComponent<PlayerSkillManager>().BlockNormalAttackRpc(false);
        _sManager.GetComponent<PlayerSkillManager>().BlockSkillsRpc(false);

        _canInstantiateRangedMinion = true;

        HealthManager.OnMelHealed -= OnMelHealed;

        _timesExtended = 0;

        base.ReturnObject();
    }
}
