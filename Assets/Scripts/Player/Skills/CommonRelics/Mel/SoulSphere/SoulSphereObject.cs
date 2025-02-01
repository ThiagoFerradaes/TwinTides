using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SoulSphereObject : NetworkBehaviour {
    SoulSphere _info;
    int _level;
    public void ActivateSkill(SoulSphere info, int level) {
        _info = info;
        _level = level;

        Debug.Log("Sphere Activate");

        StartCoroutine(Move());
    }

    IEnumerator Move() {

        float startTime = Time.time;

        while (Time.time < startTime + _info.SphereDuration) {
            transform.Translate(_info.Speed * Time.deltaTime * transform.forward);
            yield return null;
        }

        if (_level > 1) {
            Explode();
        }
        PlayersSkillPooling.Instance.ReturnObjetToQueue(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (_level == 1) {
            if (other.CompareTag("Enemy")) {
                other.GetComponent<HealthManager>().ApplyDamageOnServerRPC(_info.DamagePassingThrowEnemy, true, true);
                StopAllCoroutines();
                PlayersSkillPooling.Instance.ReturnObjetToQueue(gameObject);
            }
            else if (other.CompareTag("Maevis")) {
                other.GetComponent<HealthManager>().AddBuffToList(_info.invulnerabilityBuff);
                StopAllCoroutines();
                PlayersSkillPooling.Instance.ReturnObjetToQueue(gameObject);
            }
        }
        else {
            if (other.CompareTag("Enemy")) {
                other.GetComponent<HealthManager>().ApplyDamageOnServerRPC(_info.DamagePassingThrowEnemy, true, true);
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
        //explosion.GetComponent<>
        StartCoroutine(ExplosionTime(_info.ExplosionDuration, explosion));
    }

    IEnumerator ExplosionTime(float time, GameObject explosion) {
        explosion.SetActive(true);
        yield return new WaitForSeconds(time);
        explosion.SetActive(false);

        if (_level == 3) {
            GameObject area = PlayersSkillPooling.Instance.GetObjectFromQueue(_info.sphereAreaPreFab);
            area.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            area.SetActive(true);
            //area.GetComponent<>;
        }
        else if (_level == 4) {
            GameObject area = PlayersSkillPooling.Instance.GetObjectFromQueue(_info.sphereAreaPreFab);
            area.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            area.transform.localScale *= _info.AreaScaleLevel4;
            area.SetActive(true);
            //area.GetComponent<>;
        }

        PlayersSkillPooling.Instance.ReturnObjetToQueue(gameObject);
    }
}
