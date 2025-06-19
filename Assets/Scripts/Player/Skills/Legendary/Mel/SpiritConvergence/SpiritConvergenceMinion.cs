using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SpiritConvergenceMinion : SkillObjectPrefab {
    SpiritConvergence _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    Transform _target;
    bool _isAttacking;

    [SerializeField] bool isRanged;

    Coroutine _mainLoopCoroutine;

    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as SpiritConvergence;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
        }

        Initiate();
    }

    void Initiate() {
        Vector3 offsetDir = (transform.position - _mel.transform.position).normalized;
        Vector3 spawnPos = _mel.transform.position + (offsetDir * 1.5f);

        if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 2f, NavMesh.AllAreas)) {
            transform.position = hit.position;
        }
        else {
            transform.position = _mel.transform.position + Vector3.right * 2f; 
        }

        transform.rotation = _context.PlayerRotation;
        gameObject.SetActive(true);

        _mainLoopCoroutine = StartCoroutine(MinionBehaviorLoop());
        StartCoroutine(Duration());
    }


    IEnumerator Duration() {
        float elapsedTime = 0f;
        float duration = isRanged ? _info.RangedMinionDuration : _info.MeleeMinionDuration;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ReturnObject();
    }

    IEnumerator MinionBehaviorLoop() {
        float rangeToMel = _info.RangeToMel;
        float moveSpeed = isRanged ? _info.RangedMinionSpeed : _info.MeleeMinionSpeed;
        float attackRange = isRanged ? _info.RangedMinionAttackRange : _info.MeleeMinionAttackRange;

        while (true) {
            _target = FindClosestEnemy();

            Vector3 targetPos;

            if (_target != null) {
                float distance = Vector3.Distance(transform.position, _target.position);

                if (distance > attackRange + 0.1f) {
                    targetPos = _target.position;
                    MoveTowards(targetPos, moveSpeed);
                }
                else {
                    StopMoving();
                    Attack();
                }
            }
            else {
                float distanceToMel = Vector3.Distance(transform.position, _mel.transform.position);

                if (distanceToMel > rangeToMel + 0.1f) {
                    targetPos = _mel.transform.position;
                    MoveTowards(targetPos, moveSpeed);
                }
                else {
                    StopMoving();
                }
            }

            yield return null;
        }
    }
    [SerializeField] LayerMask floorLayer;

    void MoveTowards(Vector3 targetPos, float speed) {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;

        Vector3 nextPosition = transform.position + direction * speed * Time.deltaTime;

        if (Physics.Raycast(nextPosition + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 3f, floorLayer)) {
            nextPosition.y = hit.point.y + 1;
        }

        transform.position = nextPosition;

        if (direction != Vector3.zero) {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }




    void StopMoving() { }


    Transform FindClosestEnemy() {
        float radius = isRanged ? _info.RangedMinionRangeToFindEnemy : _info.MeleeMinionRangeToFindEnemy;
        Collider[] enemies = Physics.OverlapSphere(_mel.transform.position, radius);

        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider enemy in enemies) {
            if (!enemy.CompareTag("Enemy")) continue;
            if (!enemy.TryGetComponent(out HealthManager health)) continue;
            if (health.ReturnDeathState() || !enemy.gameObject.activeInHierarchy) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }
        return closestEnemy;
    }

    void Attack() {
        if (_isAttacking || _target == null) return;
        _isAttacking = true;

        Vector3 direction = _target.position;
        direction.y = transform.position.y;
        transform.LookAt(direction);

        if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Mel) {
            int variant = isRanged ? 4 : 3;
            int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
            SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);
            PlayerSkillPooling.Instance.RequestInstantiateNoChecksRpc(skillId, newContext, _level, variant);
        }

        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown() {
        float cooldown = isRanged ? _info.RangedMinionAttackCooldown : _info.MeleeMinionAttackCooldown;
        yield return new WaitForSeconds(cooldown);
        _isAttacking = false;
    }

    public override void ReturnObject() {
        if (_mainLoopCoroutine != null) {
            StopCoroutine(_mainLoopCoroutine);
            _mainLoopCoroutine = null;
        }

        _isAttacking = false;
        _target = null;

        base.ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) { }
}

