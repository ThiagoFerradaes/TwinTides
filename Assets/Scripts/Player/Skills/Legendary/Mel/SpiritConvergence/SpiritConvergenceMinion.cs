using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SpiritConvergenceMinion : SkillObjectPrefab {
    SpiritConvergence _info;
    int _level;
    SkillContext _context;
    GameObject _mel;
    NavMeshAgent _agent;
    Transform _target;

    Coroutine _followMelCoroutine, _followEnemyCoroutine;

    [SerializeField] bool isRanged;


    public override void ActivateSkill(Skill info, int skillLevel, SkillContext context) {
        _info = info as SpiritConvergence;
        _level = skillLevel;
        _context = context;

        if (_mel == null) {
            _mel = PlayerSkillPooling.Instance.MelGameObject;
            _agent = GetComponent<NavMeshAgent>();
        }

        Initiate();
    }

    void Initiate() {

        transform.SetPositionAndRotation(_context.Pos, _context.PlayerRotation);

        gameObject.SetActive(true);

        
        StartCoroutine(Duration());
        SearchEnemy();
    }

    IEnumerator Duration() {
        float elapsedTime = 0;
        float duration = isRanged ? _info.RangedMinionDuration : _info.MeleeMinionDuration;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        End();
    }

    void SearchEnemy() {

        _target = FindClosestEnemy();

        if (_target != null) {
            _followEnemyCoroutine ??= StartCoroutine(FollowEnemy());
        }
        else _followMelCoroutine ??= StartCoroutine(FollowMel());

    }

    IEnumerator FollowEnemy() {
        float range = isRanged ? _info.RangedMinionAttackRange : _info.MeleeMinionAttackRange;

        while (_target != null && Vector3.Distance(transform.position, _target.position) > range) {

            _agent.isStopped = false;
            _agent.speed = isRanged ? _info.meleeMinionSpeed : _info.rangedMinionSpeed;
            _agent.destination = _target.position;

            yield return null;
        }

        _agent.isStopped = true;
        _agent.SetDestination(_agent.transform.position);

        if (_target != null) Attack();

        else {
            float duration = isRanged ? _info.RangedMinionAttackCooldown : _info.MeleMinionAttackCooldown;
            yield return new WaitForSeconds(duration);
            _followEnemyCoroutine = null;
            SearchEnemy();
        }
    }

    IEnumerator FollowMel() {
        while (_target == null) {
            if (Vector3.Distance(transform.position, _mel.transform.position) > _info.RangeToMel) {
                _agent.isStopped = false;
                _agent.speed = isRanged ? _info.meleeMinionSpeed : _info.rangedMinionSpeed;
                _agent.destination = _mel.transform.position;
            }
            else {
                _agent.isStopped = true;
                _agent.SetDestination(_agent.transform.position);
            }

            yield return new WaitForSeconds(_info.CooldownForSearchEnemy);
            SearchEnemy();
        }

        _followMelCoroutine = null;
    }

    Transform FindClosestEnemy() {
        float radius = isRanged ? _info.MeleeMinionRangeToFindEnemy : _info.RangedMinionRangeToFindEnemy;
        Collider[] enemies = Physics.OverlapSphere(_mel.transform.position, radius);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider enemy in enemies) {
            if (enemy.CompareTag("Enemy")) {
                enemy.TryGetComponent<HealthManager>(out HealthManager health);
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < closestDistance && !health.ReturnDeathState()) {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }
        }

        return closestEnemy;
    }

    void Attack() {

        int skillId = PlayerSkillConverter.Instance.TransformSkillInInt(_info);
        SkillContext newContext = new(transform.position, transform.rotation, _context.SkillIdInUI);

        Vector3 direction = _target.position;
        direction.y = 0;
        transform.LookAt(direction);

        if (isRanged) {
            if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Mel)
                PlayerSkillPooling.Instance.RequestInstantiateNoChecksRpc(skillId, newContext, _level, 4);
        }
        else {
            if (LocalWhiteBoard.Instance.PlayerCharacter == Characters.Mel)
                PlayerSkillPooling.Instance.RequestInstantiateNoChecksRpc(skillId, newContext, _level, 3);
        }

        StartCoroutine(AttackCooldown());

    }

    IEnumerator AttackCooldown() {
        float duration = isRanged ? _info.RangedMinionAttackCooldown : _info.MeleMinionAttackCooldown;

        yield return new WaitForSeconds(duration);

        _followEnemyCoroutine = null;

        SearchEnemy();

    }
    void End() {

        StopAllCoroutines();

        _followMelCoroutine = null; _followEnemyCoroutine = null; _target = null;

        ReturnObject();
    }

    public override void StartSkillCooldown(SkillContext context, Skill skill) {
        return;
    }
}
