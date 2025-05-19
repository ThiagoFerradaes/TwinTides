using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBeardCrossAttackField : BlackBeardAttackPrefab {
    BlackBeardCrossAttackSo _info;
    Vector3 pos;

    HashSet<HealthManager> _listOfPlayers = new();

    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        if (_info == null) _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardCrossAttackSo;

        pos = position;

        DefinePosition();

    }

    private void DefinePosition() {
        Vector3 startPos = parent.GetComponent<BlackBeardMachineState>().CenterOfArena.position;
        Vector3 finalPos = pos;

        Vector3 center = (startPos + finalPos) / 2;

        center.y = GetFloorHeight(center);

        Vector3 directon = (finalPos - startPos).normalized;

        float length = Vector3.Distance(startPos, finalPos);

        transform.localScale = new Vector3(_info.FieldSize.x, _info.FieldSize.y, length);

        transform.SetPositionAndRotation(center, Quaternion.LookRotation(directon));

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

        End();
    }

    public override void End() {
        _listOfPlayers.Clear();

        base.End();
    }

    IEnumerator DamageRoutine() {
        while (true) {
            yield return new WaitForSeconds(_info.TimeBetweenDamage);
            foreach(var player in _listOfPlayers) {
                player.DealDamage(_info.FieldDamage, false, true);
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
