using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBeardBullets : EnemyAttackPrefab
{
    BlackBeardBulletsAttackSO _info;
    HashSet<HealthManager> _listOfPlayers = new();
    Vector3 pos;

    HealthManager health;
    bool isStronger;

    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        if(_info == null)_info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardBulletsAttackSO;
        if (health == null) health = parent.GetComponent<HealthManager>();

        isStronger = health.ReturnCurrentHealth() < (health.ReturnMaxHealth() / 2);
        pos = position;

        SetPosition();
    }

    void SetPosition() {

        transform.localScale = isStronger? Vector3.one * _info.BulletSizeStronger :Vector3.one * _info.BulletsSize;

        transform.position = pos;

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {

        float durartion = isStronger ? _info.DurationStronger : _info.Duration;

        StartCoroutine(DamageRoutine());

        yield return new WaitForSeconds(durartion);

        _listOfPlayers.Clear();

        End();
    }

    IEnumerator DamageRoutine() {
        while (true) {
            foreach (var player in _listOfPlayers) {
                player.DealDamage(_info.Damage, true, true);
            }

            yield return new WaitForSeconds(_info.TimeBetweenDamages);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        _listOfPlayers.Add(health);
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        _listOfPlayers.Remove(health);
    }
}
