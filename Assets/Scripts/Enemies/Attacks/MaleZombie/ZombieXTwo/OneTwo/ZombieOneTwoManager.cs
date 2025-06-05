using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class ZombieOneTwoManager : EnemyAttackPrefab
{
    ZombieOneTwo _info;

    EventInstance sound;

    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieOneTwo;

        parentContext.Blackboard.IsAttacking = true;

        gameObject.SetActive(true);

        float distanceToPlayer = Vector3.Distance(parent.transform.position, parentContext.Blackboard.Target.position);

        if (distanceToPlayer >= _info.distanceToJump) Jump();
        else Attack();
    }

    void Jump() {

        if (!_info.JumpSound.IsNull) {
            sound = RuntimeManager.CreateInstance(_info.JumpSound);
            RuntimeManager.AttachInstanceToGameObject(sound, parent);
            sound.start();
        }

        Vector3 playerPosition = parentContext.Blackboard.Target.position;
        parent.transform.DOJump(playerPosition, _info.jumpHeight, 1, _info.jumpDuration).OnComplete(() => {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent);
            EndOfAttack();
        });
    }

    void Attack() {
        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, parent);
        EndOfAttack();
    }

    void EndOfAttack() {
        parentContext.Blackboard.IsAttacking = false;
        parentContext.Blackboard.CanAttack = false;
        parentContext.Blackboard.Cooldowns[_info.ListOfAttacksNames[0]] = _info.cooldown;
        End();
    }

    public override void End() {

        if (sound.isValid()) {
            sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            sound.release();
        }

        base.End();
    }
}
