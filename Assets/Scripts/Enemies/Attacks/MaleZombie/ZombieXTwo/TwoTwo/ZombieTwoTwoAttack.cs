using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class ZombieTwoTwoAttack : EnemyAttackPrefab {
    ZombieTwoTwo _info;
    bool playerColision;

    EventInstance sound;

    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieTwoTwo;

        transform.localScale = new Vector3(_info.hitBoxSize, 0.1f, _info.hitBoxSize);

        if (transform.parent == null) {
            transform.SetParent(parent.transform);
        }

        transform.localPosition = Vector3.zero;

        gameObject.SetActive(true);

        parentContext.Blackboard.IsAttacking = true;

        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine() {

        Vector3 direction = (parentContext.Blackboard.Target.position - parent.transform.position).normalized;

        float dashDuration = _info.dashDistance / _info.dashSpeed;

        float timer = 0;

        if (!_info.DashSound.IsNull) {
            sound = RuntimeManager.CreateInstance(_info.DashSound);
            RuntimeManager.AttachInstanceToGameObject(sound, parent);
            sound.start();
        }

        parentContext.Anim.SetTrigger("StrongAttack");
        float enterAnimTimeout = 1f;
        float animtimer = 0f;

        while (parentContext.Anim.IsInTransition(0)) {
            yield return null;
            animtimer += Time.deltaTime;
            if (animtimer > enterAnimTimeout) {
                Debug.LogWarning("Transição para animação nunca começou.");
                break;
            }
        }

        animtimer = 0f;
        AnimatorStateInfo stateInfo = parentContext.Anim.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("StrongAttack")) {
            yield return null;
            animtimer += Time.deltaTime;
            stateInfo = parentContext.Anim.GetCurrentAnimatorStateInfo(0);
            if (animtimer > enterAnimTimeout) {
                Debug.LogWarning("Animação correta nunca entrou. Cancelando CryRoutine.");
                break;
            }
        }

        while (timer < dashDuration && !playerColision) {
            parent.transform.position += (_info.dashSpeed * Time.deltaTime * direction);
            timer += Time.deltaTime;
            yield return null;
        }

        AttackEnd();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        playerColision = true;

        health.DealDamage(_info.attackDamage, true, true);

        if (!other.TryGetComponent<MovementManager>(out MovementManager movement)) return;

        movement.StunWithTime(_info.stunTime);

        if (!_info.AttackSound.IsNull) RuntimeManager.PlayOneShot(_info.AttackSound, transform.position);
    }

    void AttackEnd() {
        playerColision = false;

        parentContext.Blackboard.IsAttacking = false;

        parentContext.Blackboard.CanAttack = false;

        parentContext.Blackboard.Cooldowns[_info.ListOfAttacksNames[0]] = _info.attackCooldown;

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
