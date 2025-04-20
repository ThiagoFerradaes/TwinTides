using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomAuraObject : SkillObjectPrefab {
    PhantomAura _info;
    int _level;
    SkillContext _context;
    GameObject _mel;

    bool _canDamage;

    List<HealthManager> _listOfEnemies = new();

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as PhantomAura;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
        }

        DefineSizeAndParent();
    }

    void DefineSizeAndParent() {

        transform.localScale = _level < 4 ? _info.AuraSize : _info.AuraSizeLevel4;

        transform.SetParent(_mel.transform);

        transform.SetLocalPositionAndRotation(Vector3.zero, _context.PlayerRotation);

        gameObject.SetActive(true);

        StartCoroutine(DamageTimer());

        StartCoroutine(Duration());
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager enemyHealth)) return;

        if (!_listOfEnemies.Contains(enemyHealth)) _listOfEnemies.Add(enemyHealth);
    }

    private void OnTriggerExit(Collider other) {

        if (!other.CompareTag("Enemy")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager enemyHealth)) return;

        if (_listOfEnemies.Contains(enemyHealth)) _listOfEnemies.Remove(enemyHealth);
    }

    private void HealPlayer(float damage) {
        if (!_mel.TryGetComponent<HealthManager>(out HealthManager health)) return;

        float percentOfHealing = (_info.HealingPercent / 100) * damage;

        health.HealServerRpc(percentOfHealing, true);
    }

    IEnumerator DamageTimer() {
        while (true) {
            yield return new WaitForSeconds(_info.DamageInterval);

            foreach (var enemyHealth in _listOfEnemies) {
                float damage = _level < 4 ? _mel.GetComponent<DamageManager>().ReturnTotalAttack(_info.Damage) :
                            _mel.GetComponent<DamageManager>().ReturnTotalAttack(_info.DamageLevel4);

                bool enemieDead = enemyHealth.ReturnDeathState();

                if (!enemieDead) {
                    enemyHealth.DealDamage(damage, true, true);

                    if (_level > 1) HealPlayer(damage);
                }
            }
        }
    }

    IEnumerator Duration() {
        float duration = _level < 4 ? _info.Duration : _info.DurationLevel4;

        yield return new WaitForSeconds(duration);

        End();
    }


    void End() {
        _listOfEnemies.Clear();

        ReturnObject();
    }
}
