using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBeardChain : BlackBeardAttackPrefab
{
    BlackBeardAnchorAttackSO _info;
    Vector3 pos;

    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        if (_info == null) _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardAnchorAttackSO;

        pos = position;

        DefinePosition();

    }

    private void DefinePosition() {
        Vector3 startPos = parent.transform.forward * _info.AnchorOffset;
        Vector3 finalPos = pos;

        Vector3 center = (startPos + finalPos) / 2;

        Vector3 directon = (finalPos - startPos).normalized;

        float length = Vector3.Distance(startPos, finalPos);

        transform.localScale = new Vector3(_info.ChainSize.x, _info.ChainSize.y, length);

        transform.SetPositionAndRotation(center, Quaternion.LookRotation(directon));

        transform.SetParent(parent.transform);

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {

        yield return new WaitForSeconds(_info.RotationDuration);

        transform.SetParent(null);

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.DealDamage(_info.ChainDamage, true, true);

    }
}
