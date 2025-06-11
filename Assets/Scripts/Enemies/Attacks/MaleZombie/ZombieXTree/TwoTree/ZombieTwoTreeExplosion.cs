using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.TerrainTools;

public class ZombieTwoTreeExplosion : EnemyAttackPrefab
{
    ZombieTwoTree _info;

    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieTwoTree;

        parentContext.Blackboard.IsAttacking = true;

        SetPosition(position);
    }

    void SetPosition(Vector3 position) {
        transform.position = position;

        transform.localScale = _info.explosionSize * Vector3.one;

        gameObject.SetActive(true);

        if (!_info.ExplosionSound.IsNull) RuntimeManager.PlayOneShot(_info.ExplosionSound, transform.position);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.explosionDuration);

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.DealDamage(_info.explosionDamage, true, true);

        health.AddDebuffToList(_info.poisonDebuff);
    }
}
