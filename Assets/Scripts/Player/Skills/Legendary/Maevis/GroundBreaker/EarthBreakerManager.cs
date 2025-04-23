using System.Collections;
using UnityEngine;

public class EarthBreakerManager : SkillObjectPrefab {
    EarthBreaker _info;
    SkillContext _context;
    GameObject _maevis;
    [HideInInspector] public int _amountOfImpactsSummoned;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as EarthBreaker;
        _context = context;
        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        SetPosition();
    }

    void SetPosition() {
        _amountOfImpactsSummoned = 0;

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }


    IEnumerator Duration() {
        for (int i = 0; i < _info.AmountOfImpacts; i++) {
            SummonImpact();
            yield return new WaitForSeconds(_info.CooldownBetweenEachImpact);
        }

        End();
    }
    void SummonImpact() {

        if (LocalWhiteBoard.Instance.PlayerCharacter != Characters.Maevis) return;

        _amountOfImpactsSummoned++;

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);

        Vector3 direction = _context.PlayerRotation * Vector3.forward;
        Vector3 newPos = (_context.Pos) + (direction * _info.InicialImpactSize.z * _amountOfImpactsSummoned);
        SkillContext newContext = new(newPos, _context.PlayerRotation, _context.SkillIdInUI);

        PlayerSkillPooling.Instance.RequestInstantiateNoChecksRpc(skillId, newContext, 1, 1);
    }

    void End() {
        ReturnObject();
    }

}
