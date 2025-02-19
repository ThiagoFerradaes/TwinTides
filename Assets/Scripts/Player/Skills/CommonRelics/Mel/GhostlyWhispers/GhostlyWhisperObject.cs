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
            _mel = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        DefinePosition();
    }

    void DefinePosition() {
        transform.SetParent(_mel.transform);

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

        DefineAmountOfPuddles();

        gameObject.SetActive(true);

        InstantiatePuddle(_context);

        StartCoroutine(Duration());
    }

    void InstantiatePuddle(SkillContext instantiateContext) {
        amountOfPuddles--;
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        PlayerSkillPooling.Instance.InstantiateAndSpawnNoCheckRpc(skillId, instantiateContext, _level, 1);
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

        End();
    }

    void End() {
        amountOfPuddles = 0;
        _mel.GetComponent<PlayerSkillManager>().StartCooldown(_context.SkillIdInUI, _info);
        ReturnObject();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public override void AddStackRpc() {
        SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);
        InstantiatePuddle(newContext);
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
