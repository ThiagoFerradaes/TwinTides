using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieOneTreeToxicCloud : EnemyAttackPrefab
{
    ZombieOneTree _info;

    HashSet<HealthManager> _listOfPlayers = new();
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieOneTree;

        parentContext.Blackboard.IsAttacking = true;

        SetParentAndPosition();
    }

    void SetParentAndPosition() {
        transform.SetParent(parent.transform);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        transform.localScale = Vector3.one;

        float sizeI = _info.toxicCloudInicialSize;

        float increase = sizeI + (sizeI * parentContext.Blackboard.CurrentComboIndex * _info.toxicCloudSizeIncreasePercent / 100);

        float realIncrease = Mathf.Clamp(increase, sizeI, _info.toxicCloudMaxSize);

        transform.localScale = Vector3.one * realIncrease;

        gameObject.SetActive(true);

        parentContext.Blackboard.CurrentComboIndex++;

        FakeEnding();

        StartCoroutine(Duration());
    }

    void FakeEnding() {
        parentContext.Blackboard.IsAttacking = false;

        parentContext.Blackboard.CanAttack = false;

        parentContext.Blackboard.Cooldowns[_info.ListOfAttacksNames[0]] = _info.toxicCloudDuration + _info.toxicCloudCooldown;
    }

    IEnumerator Duration() {
        float timer = 0;
        float poisonTimer = 0;

        while (timer < _info.toxicCloudDuration) {
            timer += Time.deltaTime;
            poisonTimer += Time.deltaTime;

            if (poisonTimer >= _info.timeBetweenToxicAplication) {
                foreach (var player in _listOfPlayers) {
                    player.AddDebuffToList(_info.toxicDebuff);
                }

                poisonTimer = 0;
            }

            yield return null;
        }

        _listOfPlayers.Clear();

        End();
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
