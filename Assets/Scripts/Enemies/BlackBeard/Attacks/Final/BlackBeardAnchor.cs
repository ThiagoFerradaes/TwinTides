using System.Collections;
using UnityEngine;

public class BlackBeardAnchor : BlackBeardAttackPrefab {
    BlackBeardAnchorAttackSO _info;
    Vector3 pos;
    Vector3 _direction;
    HealthManager health;
    bool isStronger;
    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        if (_info == null) _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardAnchorAttackSO;

        if (health == null) health = parent.GetComponent<HealthManager>();

        pos = position;

        isStronger = health.ReturnCurrentHealth() < (health.ReturnMaxHealth() / 2);

        DefinePosition();

    }

    private void DefinePosition() {
        transform.localScale = _info.AnchorSize;

        _direction = (pos - parent.transform.position).normalized;

        transform.SetPositionAndRotation(pos, Quaternion.LookRotation(_direction));

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        float duration = _info.AnchorRange / _info.AnchorSpeed;

        float timer = 0;

        Vector3 fowardDirection = transform.forward;

        while (timer < duration) {
            timer += Time.deltaTime;
            transform.position += _info.AnchorSpeed * Time.deltaTime * fowardDirection;
            yield return null;
        }

        if (isStronger) {
            transform.SetParent(parent.transform);
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent, transform.position);
            yield return new WaitForSeconds(_info.RotationDuration);
            transform.SetParent(null);
        }
        else {
            yield return new WaitForSeconds(_info.TimeBetweenAttacks);
        }

        Vector3 returnDirection = (parent.transform.position - transform.position).normalized;
        timer = 0;

        while (timer < duration) {
            timer += Time.deltaTime;
            transform.position += _info.AnchorSpeed * Time.deltaTime * returnDirection;
            yield return null;
        }


        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.DealDamage(_info.AnchorDamage, true, true);

        if (!other.TryGetComponent<MovementManager>(out MovementManager move)) return;

        move.StunWithTime(_info.AnchorStunTime);
    }
}
