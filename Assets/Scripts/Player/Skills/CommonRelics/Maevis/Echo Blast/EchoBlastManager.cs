using System.Collections;
using UnityEngine;

public class EchoBlastManager : SkillObjectPrefab
{
    EchoBlast _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as EchoBlast;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        EchoBlastStunExplosion.OnSecondaryExplosion += EchoBlastStunExplosion_OnSecondaryExplosion;

        gameObject.SetActive(true);

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, _context, _level, 1);

        StartCoroutine(Duration());
    }

    private void EchoBlastStunExplosion_OnSecondaryExplosion(object sender, EchoBlastStunExplosion.ExplosionPosition e) {
        StartCoroutine(SecondaryExplosion(e.context));
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.ManagerDuration);

        ReturnObject();
    }

    IEnumerator SecondaryExplosion(SkillContext context) {
        for (int i = 0; i < _info.ExplosionAmountLevel3; i++) {
            yield return new WaitForSeconds(_info.TimeBetweenEachExplosion);
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId, context, _level, 3);
        }
    }
}
