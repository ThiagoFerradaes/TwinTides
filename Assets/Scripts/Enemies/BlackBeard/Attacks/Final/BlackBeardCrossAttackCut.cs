using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class BlackBeardCrossAttackCut : BlackBeardAttackPrefab
{
    BlackBeardCrossAttackSo _info;
    Vector3 pos;
    HealthManager health;
    bool isStronger;

    EventInstance sound;
    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        if(_info == null) _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardCrossAttackSo;

        if (health == null) health = parent.GetComponent<HealthManager>();

        pos = position;

        isStronger = health.ReturnCurrentHealth() < (health.ReturnMaxHealth() / 2);

        DefinePosition();

    }

    private void DefinePosition() {
        Vector3 directon = (pos - parent.transform.position).normalized;

        transform.localScale = _info.CutSize;

        transform.SetPositionAndRotation(pos,Quaternion.LookRotation(directon));

        gameObject.SetActive(true);

        if (!_info.CutSound.IsNull) {
            sound = RuntimeManager.CreateInstance(_info.CutSound);
            RuntimeManager.AttachInstanceToGameObject(sound, this.gameObject);
            sound.start();
        }

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        float speed = isStronger ? _info.CutSpeedStronger : _info.CutSpeed;
        float duration = _info.CutRange / speed;

        float timer = 0;

        Vector3 direction = transform.forward;

        while (timer < duration) {
            timer += Time.deltaTime;
            transform.position += speed * Time.deltaTime * direction;
            yield return null;
        }

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent, transform.position);
        End();
    }

    public override void End() {

        if (sound.isValid()) {
            sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            sound.release();
        }

        base.End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float damage = isStronger ? _info.CutDamageStronger : _info.CutDamage;

        health.DealDamage(damage, true, true);
    }
}
