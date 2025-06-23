using UnityEngine;
using FMODUnity;
using DG.Tweening;
using System.Collections;

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
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        gameObject.SetActive(true);

        Vector3 jumpTarget = GetTargetPosition();
        StartCoroutine(SafeJumpCoroutine(jumpTarget, _info.JumpDuration, _info.JumpSpeed));

        if (!_info.JumpSound.IsNull) {
            RuntimeManager.PlayOneShot(_info.JumpSound, _maevis.transform.position);
        }
    }

    private Vector3 GetTargetPosition() {
        PlayerController controller = _maevis.GetComponent<PlayerController>();
        Vector3 targetPosition;

        if (controller != null && controller.gameObject.activeInHierarchy) {
            if (Vector3.Distance(_maevis.transform.position, controller.mousePos) < _info.JumpMaxRange)
                targetPosition = controller.mousePos;
            else
                targetPosition = _maevis.transform.position + _maevis.transform.forward * _info.JumpMaxRange;
        }
        else {
            targetPosition = _maevis.transform.position + _maevis.transform.forward * _info.JumpMaxRange;
        }

        targetPosition.y = GetFloorHeight(targetPosition) + 1f;
        return targetPosition;
    }

    IEnumerator SafeJumpCoroutine(Vector3 targetPosition, float duration, float jumpHeight) {
        anim.SetBool("DreadFall", true);

        Vector3 startPos = _maevis.transform.position;
        float elapsed = 0f;

        while (elapsed < duration) {
            float t = elapsed / duration;
            float height = 4f * jumpHeight * t * (1 - t); // curva parabólica

            Vector3 horizontalPos = Vector3.Lerp(startPos, targetPosition, t);
            Vector3 nextPos = horizontalPos + Vector3.up * height;

            // Verificação de colisão com parede
            Vector3 dir = nextPos - _maevis.transform.position;
            float dist = dir.magnitude;

            if (Physics.Raycast(_maevis.transform.position, dir.normalized, out RaycastHit hit, dist, LayerMask.GetMask("Wall"))) {
                // Colidiu com parede → parar no ponto de colisão
                targetPosition = hit.point;
                break;
            }

            _maevis.transform.position = nextPos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        _maevis.transform.position = targetPosition;

        anim.SetBool("DreadFall", false);
        RecieveShield();
        Explode();
        End();
    }

    float GetFloorHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor")))
            return hit.point.y + 0.1f;

        return position.y;
    }

    private void RecieveShield() {
        if (_maevis.TryGetComponent<HealthManager>(out var health)) {
            health.ApplyShield(_info.AmountOfShiled, _info.ShieldDuration, true);
        }
    }

    private void Explode() {
        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Maevis)
            return;

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);
        PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, newContext, _level, 1);
    }

    void End() {
        ReturnObject();
    }
}

