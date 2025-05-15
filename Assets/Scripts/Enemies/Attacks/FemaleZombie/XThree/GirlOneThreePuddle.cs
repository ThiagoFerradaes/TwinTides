using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlOneThreePuddle : EnemyAttackPrefab
{
    GirlOneThree _info;
    HashSet<HealthManager> _listOfPlayers = new();

    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as GirlOneThree;

        parentContext.Blackboard.IsAttacking = true;

        parentContext.Blackboard.CurrentComboIndex++;

        SetPosition();
    }

    void SetPosition() {
        float sizeMultiplier = _info.puddleSize + (_info.puddleSize * (parentContext.Blackboard.CurrentComboIndex - 1) * _info.puddleSizeIncreasePercent / 100);
        float realSize = Mathf.Clamp(sizeMultiplier, _info.puddleSize, _info.puddleMaxSize);

        transform.localScale = new Vector3(realSize, 0.1f, realSize);


        Vector3 pos = GetRandomPointNearPlayer();

        pos.y = GetFloorHeight(pos);

        transform.position = pos;

        gameObject.SetActive(true);

        StartCoroutine(Duration());

        EndOfAttack();
    }

    Vector3 GetRandomPointNearPlayer() {
        Vector2 randomCircle = Random.insideUnitCircle * _info.puddleOffSett;
        Vector3 offset = new(randomCircle.x, 0f, randomCircle.y);
        return parentContext.Blackboard.Target.position + offset;
    }

    float GetFloorHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) return hit.point.y + 0.1f;
        return position.y;
    }

    IEnumerator Duration() {
        float durationMultiplier = _info.puddleDuration + (_info.puddleDuration * (parentContext.Blackboard.CurrentComboIndex - 1) * _info.puddleDurationIncreasePercent / 100);
        float Realduration = Mathf.Clamp(durationMultiplier, _info.puddleDuration, _info.puddleMaxDuration);

        StartCoroutine(DamageRoutine());

        yield return new WaitForSeconds(Realduration);

        End();
    }

    IEnumerator DamageRoutine() {
        while (true) {
            foreach (var player in _listOfPlayers) {
                player.DealDamage(_info.puddleDamage, true, true);
            }

            yield return new WaitForSeconds(_info.timeBetweenDamages);
        }
    }

    void EndOfAttack() {
        parentContext.Blackboard.IsAttacking = false;
        parentContext.Blackboard.CanAttack = false;
        parentContext.Blackboard.Cooldowns[_info.ListOfAttacksNames[0]] = _info.cooldown;
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
