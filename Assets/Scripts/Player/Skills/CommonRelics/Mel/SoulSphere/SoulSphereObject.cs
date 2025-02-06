using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SoulSphereObject : SkillObjectPrefab {
    SoulSphere _info;
    int _level;
    SkillContext _skillContext;
    public override void ActivateSkill(Skill info, int level, SkillContext context) {
        _info = info as SoulSphere;
        _level = level;
        _skillContext = context;

        Debug.Log("Sphere Activate");

        transform.SetPositionAndRotation(context.PlayerPosition, context.PlayerRotation);
        gameObject.SetActive(true);

        StartCoroutine(Move());
    }

    IEnumerator Move() {

        float startTime = Time.time;

        while (Time.time < startTime + _info.SphereDuration) {
            transform.Translate(_info.SphereSpeed * Time.deltaTime * transform.forward);
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
            if (other.CompareTag("Enemy")) {
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
        GameObject explosion = transform.GetChild(0).gameObject;
        StartCoroutine(ExplosionTime(_info.ExplosionDuration, explosion));
    }

    IEnumerator ExplosionTime(float time, GameObject explosion) {
        explosion.SetActive(true);
        yield return new WaitForSeconds(time);
        explosion.SetActive(false);

        if (_level >= 3) {
            int skillID = PlayersSkillPooling.Instance.TransformSkillInInt(_info);
            PlayersSkillPooling.Instance.InstanciateObjectRpc(skillID,_skillContext, _level, 1);
        }

        ReturnObject();
    }

    void ReturnObject() {
        if (IsServer) {
            PlayersSkillPooling.Instance.ReturnObjetToQueue(gameObject);
        }
    }
}
