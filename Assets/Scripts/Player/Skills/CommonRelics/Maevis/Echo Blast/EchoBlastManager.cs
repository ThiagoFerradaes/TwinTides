using System.Collections;
using UnityEngine;

public class EchoBlastManager : SkillObjectPrefab {
    EchoBlast _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    Animator anim;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as EchoBlast;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            anim = _maevis.GetComponentInChildren<Animator>();
        }

        EchoBlastStunExplosion.OnSecondaryExplosion += EchoBlastStunExplosion_OnSecondaryExplosion;

        gameObject.SetActive(true);

        anim.SetTrigger("EchoBlast");

        if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, _context, _level, 1);
        }

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
            if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Maevis) {
                int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
                PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId, context, _level, 3);
            }
        }
    }
}
