using System.Collections;
using UnityEngine;

public class MaevisNormalAttackObject : SkillObjectPrefab {
    MaevisNormalAttack _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as MaevisNormalAttack;
        _level = skillLevel;
        _context = context;

        DefineParentAndPosition();
    }

    void DefineParentAndPosition() {
        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
        }

        transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(0,0,0));    


    }

    //IEnumerator RotateRight() {
    //    float startAngle = transform.rotation.eulerAngles.y;

    //}
}
