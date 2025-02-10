using System.Collections;
using UnityEngine;

public class SoulSphereObject : SkillObjectPrefab {
    SoulSphere _info;
    int _level;
    SkillContext _skillContext;
    public override void ActivateSkill(Skill info, int level, SkillContext context) {
        _info = info as SoulSphere;
        _level = level;
        _skillContext = context;

        transform.SetPositionAndRotation(_skillContext.PlayerPosition, _skillContext.PlayerRotation);
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
            if (other.CompareTag("Enemy") && IsServer) {
                other.GetComponent<HealthManager>().ApplyDamageOnServerRPC(_info.DamagePassingThroughEnemy, true, true);
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
            if (other.CompareTag("Enemy") && IsServer) {
                other.GetComponent<HealthManager>().ApplyDamageOnServerRPC(_info.DamagePassingThroughEnemy, true, true);
            }
            else if (other.CompareTag("Maevis")) {
                other.GetComponent<HealthManager>().AddBuffToList(_info.invulnerabilityBuff);
                StopAllCoroutines();
                Explode();
            }
        }
    }

    void Explode() {
        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        SkillContext context = new(transform.position, transform.rotation);
        PlayerSkillPooling.Instance.InstantiateAndSpawnRpc(skillId,context, _level, 1);
    }
}
