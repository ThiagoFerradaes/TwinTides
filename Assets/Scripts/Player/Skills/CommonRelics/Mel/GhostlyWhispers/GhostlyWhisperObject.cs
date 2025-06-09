using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GhostlyWhisperObject : SkillObjectPrefab {
    GhostlyWhispers _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    int amountOfPuddles;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as GhostlyWhispers;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
        }

        DefinePosition();
    }

    void DefinePosition() {

        transform.SetParent(_mel.transform);

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        DefineAmountOfPuddles();

        gameObject.SetActive(true);

        InstantiatePuddle();

        if (_level > 1) StartCoroutine(Duration());
        else ReturnObject();
    }

    void InstantiatePuddle() {

        amountOfPuddles--;

        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Mel) return;

        Vector3 skillPos;

        skillPos.y = GetFloorHeight(_context.Pos);

        PlayerController controller = _mel.GetComponent<PlayerController>();
        Vector3 direction = _context.PlayerRotation * Vector3.forward;
        Vector3 position = _context.Pos + (direction * _info.MaxRange);

        if (controller != null && controller.isAiming && Vector3.Distance(controller.mousePos, _mel.transform.position) < _info.MaxRange) {

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
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
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
        float elapsedTime = 0f;
        float duration;
        if (_level < 4) duration = _info.ObjectDurationLevel2;
        else duration = _info.ObjectDurationLevel4;

        while (amountOfPuddles > 0 && elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ReturnObject();
    }

    public override void ReturnObject() {
        amountOfPuddles = 0;
        if (_info.Character == LocalWhiteBoard.Instance.PlayerCharacter) {
            _mel.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, _info);
        }
        base.ReturnObject();
    }

    public override void AddStack() {
        InstantiatePuddle();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
