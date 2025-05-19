using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBeardRainBulletField : EnemyAttackPrefab
{
    BlackBeardBulletRainSO _info;
    HashSet<HealthManager> _listOfPlayers = new();
    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardBulletRainSO;

        SetPosition(position);
    }

    void SetPosition(Vector3 position) {
        position.y = GetFloorHeight(position);

        transform.position = position;

        transform.localScale = new Vector3(_info.FieldSize.x, _info.FieldSize.y, _info.FieldSize.x);

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    float GetFloorHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) return hit.point.y + 0.1f;
        return position.y;
    }

    IEnumerator Duration() {
        StartCoroutine(DamageRoutine());

        yield return new WaitForSeconds(_info.FieldDuration);

        _listOfPlayers.Clear();

        End();
    }
    IEnumerator DamageRoutine() {
        while (true) {
            yield return new WaitForSeconds(_info.TimeBetweenFieldDamage);
            foreach (var player in _listOfPlayers) {
                player.DealDamage(_info.FieldDamage, false, true);
                player.AddDebuffToList(_info.BurningDebuff);
            }
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
