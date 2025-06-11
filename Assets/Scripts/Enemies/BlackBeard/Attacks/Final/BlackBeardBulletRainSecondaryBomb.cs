using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class BlackBeardBulletRainSecondaryBomb : BlackBeardAttackPrefab {
    BlackBeardBulletRainSO _info;
    float amountOfBombsLeft;
    float _explosionMultiplier;

    EventInstance sound;

    public override void StartAttack(int enemyId, int skillId, Vector3 position, float amountOfBombs) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardBulletRainSO;

        amountOfBombsLeft = amountOfBombs - 1;

        _explosionMultiplier = _info.AmountOfSecondaryBullets - amountOfBombsLeft;

        SetPosition(position);
    }

    void SetPosition(Vector3 position) {
        transform.position = position;

        float bulletRadius = _info.SecondaryBulletSize - (_info.SecondaryBulletSize * _explosionMultiplier * _info.SecondaryBulletSizeMultiplier/100);

        transform.localScale = Vector3.one * bulletRadius;

        gameObject.SetActive(true);

        if (!_info.BounceSound.IsNull) {
            sound = RuntimeManager.CreateInstance(_info.BounceSound);
            RuntimeManager.AttachInstanceToGameObject(sound, this.gameObject);
            sound.start();
        }

        FallRoutine();
    }

    void FallRoutine() {
        Transform center = parent.GetComponent<BlackBeardMachineState>().CenterOfArena;
        Vector3 direction = (center.position - transform.position).normalized;
        Vector3 finalPosition = transform.position + direction * _info.SecondaryBulletDistanceToMainBullet;
        transform.DOJump(finalPosition, _info.SecondaryBulletJumpPower, 1, _info.SecondaryBulletJumpDuration).OnComplete(() => {

            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent, transform.position, _explosionMultiplier);

            if (amountOfBombsLeft > 0) {
                EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 4, parent, transform.position, amountOfBombsLeft);
            }

            End();
        });
    }

    public override void End() {

        if (sound.isValid()) {
            sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            sound.release();
        }

        base.End();
    }
}
