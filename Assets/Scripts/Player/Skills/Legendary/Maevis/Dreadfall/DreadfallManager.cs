using UnityEngine;
using DG.Tweening;
using System.Collections;
using FMODUnity;

public class DreadfallManager : SkillObjectPrefab {
    Dreadfall _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    Animator anim;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as Dreadfall;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            anim = _maevis.GetComponentInChildren<Animator>();
        }

        SetParentAndPosition();
    }

    private void SetParentAndPosition() {

        transform.parent = _maevis.transform;

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        gameObject.SetActive(true);

        Jump();

        if (!_info.JumpSound.IsNull) RuntimeManager.PlayOneShot(_info.JumpSound, _maevis.transform.position);
    }

    void Jump() {

        PlayerController controller = _maevis.GetComponent<PlayerController>();
        Vector3 targetPosition;

        if (controller != null && controller.gameObject.activeInHierarchy) {
            if (Vector3.Distance(_maevis.transform.position, controller.mousePos) < _info.JumpMaxRange) targetPosition = controller.mousePos;
            else targetPosition = _maevis.transform.position + _maevis.transform.forward * _info.JumpMaxRange;
        }
        else targetPosition = _maevis.transform.position + _maevis.transform.forward * _info.JumpMaxRange;

        targetPosition.y = GetFloorHeight(targetPosition) + 1f;

        if (_info.impactAnimationName != null) anim.SetBool(_info.jumpAnimationName, true);

        _maevis.transform.DOJump(targetPosition, _info.JumpSpeed, 1, _info.JumpDuration).OnComplete(() => {

            if(_info.impactAnimationName != null) anim.SetBool(_info.jumpAnimationName, false);

            if (_info.impactAnimationName != null)  anim.SetTrigger(_info.impactAnimationName);

            RecieveShield();

            Explode();

            End();
        });


    }

    float GetFloorHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) return hit.point.y + 0.1f;
        return position.y;
    }

    private void RecieveShield() {

        if (!_maevis.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.ApplyShield(_info.AmountOfShiled, _info.ShieldDuration, true);
    }

    private void Explode() {
        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Maevis) return;

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);
        PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, newContext, _level, 1);
    }
    void End() {
        ReturnObject();
    }
}
