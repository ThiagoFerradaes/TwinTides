using System;
using System.Collections;
using UnityEngine;

public class SoulSphereObject : SkillObjectPrefab {
    SoulSphere _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    public override void ActivateSkill(Skill info, int level, SkillContext context) {
        _info = info as SoulSphere;
        _level = level;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
        }

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);
        gameObject.SetActive(true);

        StartCoroutine(Move());
    }

    IEnumerator Move() {

        float startTime = Time.time;

        while (Time.time < startTime + _info.SphereDuration) {
            transform.Translate(_info.SphereSpeed * Time.deltaTime * Vector3.forward);
            yield return null;
        }

        if (_level > 1) {
            Explode();
        }
        ReturnObject();
    }

    private void OnTriggerEnter(Collider other) {
        if (_level == 1) {
            if (other.CompareTag("Enemy")) {
                float damage = _mel.GetComponent<DamageManager>().ReturnTotalAttack(_info.DamagePassingThroughEnemy);
                other.GetComponent<HealthManager>().DealDamage(damage, true, true);
                StopAllCoroutines();
                ReturnObject();
            }
            else if (other.CompareTag("Maevis")) {
                other.GetComponent<HealthManager>().AddBuffToList(_info.invulnerabilityBuff);
                StopAllCoroutines();
                ReturnObject();
            }
        }
        else {
            if (other.CompareTag("Enemy") ) {
                other.GetComponent<HealthManager>().DealDamage(_info.DamagePassingThroughEnemy, true, true);
            }
            else if (other.CompareTag("Maevis")) {
                StopAllCoroutines();
                Explode();
                ReturnObject();
            }
        }
    }

    void Explode() {
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        SkillContext context = new(transform.position, transform.rotation, _context.SkillIdInUI);
        PlayerSkillPooling.Instance.RequestInstantiateRpc(skillId,context, _level, 1);
    }
}
