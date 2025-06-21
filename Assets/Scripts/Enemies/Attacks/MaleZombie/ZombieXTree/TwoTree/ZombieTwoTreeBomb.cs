using FMOD.Studio;
using FMODUnity;
using System.Collections;

using UnityEngine;

public class ZombieTwoTreeBomb : EnemyAttackPrefab
{
    ZombieTwoTree _info;
    bool collided;

    EventInstance sound;

    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieTwoTree;

        parentContext.Blackboard.IsAttacking = true;

        SetPosition(position);
    }

    void SetPosition(Vector3 position) {
        transform.position = position;

        gameObject.SetActive(true);

        if (!_info.FallingSound.IsNull) {
            sound = RuntimeManager.CreateInstance(_info.FallingSound);
            RuntimeManager.AttachInstanceToGameObject(sound, this.gameObject);
            sound.start();
        }

        StartCoroutine(FallRoutine());
    }

    IEnumerator FallRoutine() {
        while (!collided) {
            transform.position += _info.fallSpeed * Time.deltaTime * -transform.up;
            yield return null;
        }

        collided = false;

        End();
    }

    public override void End() {

        if (sound.isValid()) {
            sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            sound.release();
        }

        base.End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Floor")) return;

        if (collided) return;

        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent, transform.position);

        collided = true;
    }
}
