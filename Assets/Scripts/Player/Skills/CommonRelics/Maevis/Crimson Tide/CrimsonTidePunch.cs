using FMODUnity;
using System.Collections;
using UnityEngine;

public class CrimsonTidePunch : SkillObjectPrefab
{
    CrimsonTide _info;
    int _level;
    SkillContext _context;
    GameObject _maevis;
    Animator anim;
    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as CrimsonTide;
        _level = skillLevel;
        _context = context;

        if (_maevis == null) {
            _maevis = PlayerSkillPooling.Instance.MaevisGameObject;
            anim = _maevis.GetComponentInChildren<Animator>();
        }

        DefinePosition();
    }

    private void DefinePosition() {
        Vector3 direction = _context.PlayerRotation * Vector3.forward;
        Vector3 position = _context.Pos + (direction * _info.PunchAreaOffSett);
        transform.SetPositionAndRotation(position, _context.PlayerRotation);

        anim.SetTrigger("CrimsonTide");

        gameObject.SetActive(true);

        if (!_info.PunchSound.IsNull) RuntimeManager.PlayOneShot(_info.PunchSound, transform.position) ;

        StartCoroutine(PunchDuration());
    }

    IEnumerator PunchDuration() {
        yield return new WaitForSeconds(_info.PunchAreaDuration);

        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.DealDamage(_info.PunchDamage, true, true);
    }
}
