using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieTreeOneSpin : EnemyAttackPrefab
{
    ZombieTreeOne _info;

    List<HealthManager> _listOfPlayers = new();
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieTreeOne;

        DefinePosition();

    }

    private void DefinePosition() {

        Vector3 pos = parent.transform.position;

        transform.SetPositionAndRotation(pos, parent.transform.rotation);

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        StartCoroutine(Damage());

        yield return new WaitForSeconds(_info.totalSpinDuration);

        _listOfPlayers.Clear();

        End();
    }

    IEnumerator Damage() {
        float timeBetweenSpins = _info.totalSpinDuration / _info.amountOfTicksPerSpin;
        for (int i = 0; i < _info.amountOfTicksPerSpin; i++) {
            foreach (var player in _listOfPlayers) {
                player.DealDamage(_info.damagePerTick, true, true);
            }

            yield return new WaitForSeconds(timeBetweenSpins);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        _listOfPlayers.Add(health);
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        _listOfPlayers.Remove(health);
    }
}
