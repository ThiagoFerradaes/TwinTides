using FMODUnity;
using System.Collections;
using UnityEngine;

public class BlackBeardShootUpBulletWarning : BlackBeardAttackPrefab {
    BlackBeardCannon _info;

    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardCannon;

        SetPosition(position);
    }

    void SetPosition(Vector3 position) {
        position.y = 0;

        transform.position = position;

        transform.localScale = new Vector3(_info.ShootUpBulletWarningSize, 0.35f, _info.ShootUpBulletWarningSize);

        gameObject.SetActive(true);

        if (!_info.WarningSound.IsNull) RuntimeManager.PlayOneShot(_info.WarningSound, transform.position);

        StartCoroutine(Duration());
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.ShootUpBulletWarningDuration);

        End();
    }
}
