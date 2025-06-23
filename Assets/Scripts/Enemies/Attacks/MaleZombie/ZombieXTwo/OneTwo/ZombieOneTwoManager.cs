using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class ZombieOneTwoManager : EnemyAttackPrefab
{
    ZombieOneTwo _info;

    EventInstance sound;

    Animator anim;

    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieOneTwo;

        parentContext.Blackboard.IsAttacking = true;

        if (anim == null) anim = parentContext.Anim;

        gameObject.SetActive(true);

        float distanceToPlayer = Vector3.Distance(parent.transform.position, parentContext.Blackboard.Target.position);

        if (distanceToPlayer >= _info.distanceToJump) StartCoroutine( Jump());
        else Attack();
    }

    IEnumerator Jump() {

        if (!_info.JumpSound.IsNull) {
            sound = RuntimeManager.CreateInstance(_info.JumpSound);
            RuntimeManager.AttachInstanceToGameObject(sound, parent);
            sound.start();
        }

        Vector3 playerPosition = parentContext.Blackboard.Target.position;

        anim.SetTrigger("Jump");
        float enterAnimTimeout = 1f;
        float timer = 0f;

        while (anim.IsInTransition(0)) {
            yield return null;
            timer += Time.deltaTime;
            if (timer > enterAnimTimeout) {
                Debug.LogWarning("Transição para animação nunca começou.");
                break;
            }
        }

        timer = 0f;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("Pulo")) {
            yield return null;
            timer += Time.deltaTime;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (timer > enterAnimTimeout) {
                Debug.LogWarning("Animação correta nunca entrou. Cancelando CryRoutine.");
                break;
            }
        }

        parent.transform.DOJump(playerPosition, _info.jumpHeight, 1, _info.jumpDuration).OnComplete(() => {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, parent);
            EndOfAttack();
        });
    }

    IEnumerator Attack() {
        anim.SetTrigger("IsAttacking");
        float enterAnimTimeout = 1f;
        float timer = 0f;

        while (anim.IsInTransition(0)) {
            yield return null;
            timer += Time.deltaTime;
            if (timer > enterAnimTimeout) {
                Debug.LogWarning("Transição para animação nunca começou.");
                break;
            }
        }

        timer = 0f;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("Ataque")) {
            yield return null;
            timer += Time.deltaTime;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (timer > enterAnimTimeout) {
                Debug.LogWarning("Animação correta nunca entrou. Cancelando CryRoutine.");
                break;
            }
        }

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
        parentContext.Blackboard.IsAttacking = false;
        parentContext.Blackboard.CanAttack = false;

        if (sound.isValid()) {
            sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            sound.release();
        }

        base.End();
    }
}
