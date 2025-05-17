using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlackBeardFinalState : BlackBeardStates {

    #region variabels
    BlackBeardCrossAttackSo _crossInfo;
    BlackBeardAnchorAttackSO _anchorInfo;
    BlackBeardBulletsAttackSO _bulletsInfo;
    BlackBeardBulletRainSO _bulletRainInfo;
    BlackBeardWaveAttackSO _waveInfo;
    HealthManager _health;
    List<BlackBeardFinalFormAttacks> _attacks;
    #endregion

    #region Initiate
    public override void StartState(BlackBeardMachineState parent) {
        base.StartState(parent);

        _parent.Lifes = 0;

        RestartState();

        JumpToCenter();
    }

    void RestartState() {
        if (_health == null) _health = _parent.Health;

        if (_crossInfo == null) _crossInfo = _parent.ListOfAttacks[3] as BlackBeardCrossAttackSo;
        if (_anchorInfo == null) _anchorInfo = _parent.ListOfAttacks[6] as BlackBeardAnchorAttackSO;
        if (_bulletsInfo == null) _bulletsInfo = _parent.ListOfAttacks[2] as BlackBeardBulletsAttackSO;
        if (_bulletRainInfo == null) _bulletRainInfo = _parent.ListOfAttacks[4] as BlackBeardBulletRainSO;
        if (_waveInfo == null) _waveInfo = _parent.ListOfAttacks[5] as BlackBeardWaveAttackSO;

        _attacks ??= new List<BlackBeardFinalFormAttacks>()
        {
            new() { Attack = BlackBeardFinalFormAttacks.FinalFormAttacks.BULLETS, Priority = 1, Cooldown = _bulletsInfo.Cooldown},
            new() { Attack = BlackBeardFinalFormAttacks.FinalFormAttacks.BULLETRAIN, Priority = 2, Cooldown = _bulletRainInfo.Cooldown },
            new() { Attack = BlackBeardFinalFormAttacks.FinalFormAttacks.CROSS, Priority = 1, Cooldown = _crossInfo.Cooldown },
            new() { Attack = BlackBeardFinalFormAttacks.FinalFormAttacks.WAVE, Priority = 3, Cooldown = _waveInfo.Cooldown },
            new() { Attack = BlackBeardFinalFormAttacks.FinalFormAttacks.ANCHOR, Priority = 2, Cooldown = _anchorInfo.Cooldown },
        };

    }

    void JumpToCenter() {
        _parent.transform.DOJump(_parent.CenterOfArena.position, 10, 1, 0.8f).OnComplete(() => {
            _parent.StartCoroutine(WaitToAtack());
        });
    }

    IEnumerator WaitToAtack() {
        yield return new WaitForSeconds(0.5f);
        Attack();
    }
    #endregion

    #region Attack Region
    void Attack() {

        BlackBeardFinalFormAttacks chosenAttack = ChooseAttack();
        if (chosenAttack == null) return;

        switch (chosenAttack.Attack) {
            case BlackBeardFinalFormAttacks.FinalFormAttacks.BULLETS:
                _parent.StartCoroutine(BulletsAttack());
                break;
            case BlackBeardFinalFormAttacks.FinalFormAttacks.BULLETRAIN:
                _parent.StartCoroutine(RainBulletAttack());
                break;
            case BlackBeardFinalFormAttacks.FinalFormAttacks.CROSS:
                _parent.StartCoroutine(CrossAttack());
                break;
            case BlackBeardFinalFormAttacks.FinalFormAttacks.WAVE:
                _parent.StartCoroutine(WaveImpactAttack());
                break;
            case BlackBeardFinalFormAttacks.FinalFormAttacks.ANCHOR:
                _parent.StartCoroutine(AnchorAttack());
                break;

        }
        chosenAttack.Use();
    }

    BlackBeardFinalFormAttacks ChooseAttack() {
        for (int priority = 1; priority <= 3; priority++) {
            var available = _attacks.Where(a => a.Priority == priority && a.IsReady).ToList();

            if (available.Count > 0) {
                return available[Random.Range(0, available.Count)];
            }
        }

        return null;
    }


    #region CrossAttack
    IEnumerator CrossAttack() {

        Vector3 centerOfArena = _parent.CenterOfArena.position;
        float distanceToCenter = Vector3.Distance(_parent.transform.position, centerOfArena);

        if (distanceToCenter > 0.5f) // margem de tolerância
{
            yield return DashToPosition(centerOfArena);
        }

        Transform nearestPlayer = FindNearestPlayer();
        if (nearestPlayer != null) {
            Vector3 lookDirection = (nearestPlayer.position - _parent.transform.position).normalized;

            lookDirection.y = 0f;

            if (lookDirection != Vector3.zero) {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                _parent.transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
            }
        }

        Vector3 center = _parent.transform.position;
        int amountOfCuts = _health.ReturnCurrentHealth() > (_health.ReturnMaxHealth() / 2) ? _crossInfo.AmountOfCuts : _crossInfo.AmountOfCutsStronger;

        float baseAngle = Mathf.Atan2(_parent.transform.forward.z, _parent.transform.forward.x) * Mathf.Rad2Deg;
        float angleStep = 360f / amountOfCuts;

        for (int i = 0; i < amountOfCuts; i++) {
            float angle = baseAngle + angleStep * i;
            float radAngles = Mathf.Deg2Rad * angle;

            float x = center.x + Mathf.Cos(radAngles) * _crossInfo.DistanceToCenter;
            float z = center.z + Mathf.Sin(radAngles) * _crossInfo.DistanceToCenter;

            Vector3 spawnPosition = new(x, center.y, z);
            EnemySkillPooling.Instance.RequestInstantiateAttack(_crossInfo, 0, _parent.gameObject, spawnPosition);
        }


        _parent.StartCoroutine(CooldownBetweenAttacks());
    }

    private IEnumerator DashToPosition(Vector3 targetPos) {
        float dashSpeed = 20f;
        float closeEnough = 0.1f;

        while (Vector3.Distance(_parent.transform.position, targetPos) > closeEnough) {
            _parent.transform.position = Vector3.MoveTowards(
                _parent.transform.position,
                targetPos,
                dashSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    private Transform FindNearestPlayer() {
        float minDist = float.MaxValue;
        Transform nearest = null;

        foreach (var player in SceneManager.ActivePlayers) {
            float dist = Vector3.Distance(_parent.transform.position, player.transform.position);
            if (dist < minDist) {
                minDist = dist;
                nearest = player.transform;
            }
        }

        return nearest;
    }
    #endregion

    #region BulletsAttack

    IEnumerator BulletsAttack() {
        float duration = IsStronger() ? _bulletsInfo.DurationStronger : _bulletsInfo.Duration;
        int amount = IsStronger() ? _bulletsInfo.AmountOfAttacksStronger : _bulletsInfo.AmountOfTimesAttack;
        for (int i = 0; i < amount; i++) {

            Transform target = FindFarthestPlayer();

            yield return DashToTarget(target.position);

            EnemySkillPooling.Instance.RequestInstantiateAttack(_bulletsInfo, 0, _parent.gameObject, _parent.transform.position);

            yield return new WaitForSeconds(duration + _bulletsInfo.TimeBetweenOneAttackAndTheNext);
        }

        _parent.StartCoroutine(CooldownBetweenAttacks());
    }
    IEnumerator DashToTarget(Vector3 targetPosition) {
        float speed = IsStronger() ? _bulletsInfo.DashSpeedStronger : _bulletsInfo.DashSpeed;

        Vector3 start = _parent.transform.position;
        Vector3 direction = (targetPosition - start).normalized;

        float distance = Vector3.Distance(start, targetPosition);

        float duration = distance / speed;
        float elapsed = 0f;

        Vector3 lookDirection = new(direction.x, 0, direction.z);
        _parent.transform.rotation = Quaternion.LookRotation(lookDirection);

        while (elapsed < duration) {
            _parent.transform.position += direction * (speed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _parent.transform.position = targetPosition;
    }
    private Transform FindFarthestPlayer() {
        float maxDist = float.MinValue;
        Transform farthest = null;

        foreach (var player in SceneManager.ActivePlayers) {
            float dist = Vector3.Distance(_parent.transform.position, player.transform.position);
            if (dist > maxDist) {
                maxDist = dist;
                farthest = player.transform;
            }
        }

        return farthest;
    }

    #endregion

    #region AnchorAttack
    IEnumerator AnchorAttack() {

        Vector3 centerOfArena = _parent.CenterOfArena.position;
        float distanceToCenter = Vector3.Distance(_parent.transform.position, centerOfArena);

        if (distanceToCenter > 0.5f) 
{
            yield return DashToPosition(centerOfArena);
        }

        if (!IsStronger()) {

            Transform farthestPlayer = FindFarthestPlayer();
            if (farthestPlayer != null) {
                Vector3 lookDirection = (farthestPlayer.position - _parent.transform.position).normalized;

                lookDirection.y = 0f;

                if (lookDirection != Vector3.zero) {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    _parent.transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
                }
            }

            Vector3 anchorPosition = _parent.transform.forward * _anchorInfo.AnchorOffset;

            EnemySkillPooling.Instance.RequestInstantiateAttack(_anchorInfo, 0, _parent.gameObject, anchorPosition);

            float attackDuration = (_anchorInfo.AnchorSpeed / _anchorInfo.AnchorRange);

            float duration = 2 * attackDuration + _anchorInfo.TimeBetweenAttacks;

            yield return new WaitForSeconds(duration);
        }

        else { 

            Vector3 southDirection = new(0f, 0f, -1f);
            Quaternion targetRotation = Quaternion.LookRotation(southDirection);
            _parent.transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);

            Vector3 anchorPosition = _parent.transform.forward * _anchorInfo.AnchorOffset;

            EnemySkillPooling.Instance.RequestInstantiateAttack(_anchorInfo, 0, _parent.gameObject, anchorPosition);

            float attackDuration = (_anchorInfo.AnchorSpeed / _anchorInfo.AnchorRange);

            yield return new WaitForSeconds(attackDuration);

            yield return Rotate(_anchorInfo.RotationDuration);

            yield return new WaitForSeconds(attackDuration);

            _parent.transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
        }

        _parent.StartCoroutine(CooldownBetweenAttacks());
    }

    IEnumerator Rotate(float duration) {
        float elapsed = 0f;

        float rotationSpeed = _anchorInfo.AnchorRotationSPeed; 

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            _parent.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

            yield return null;
        }
    }


    #endregion

    #region WaveImpactAttack
    IEnumerator WaveImpactAttack() {

        Vector3 centerOfArena = _parent.CenterOfArena.position;
        float distanceToCenter = Vector3.Distance(_parent.transform.position, centerOfArena);

        if (distanceToCenter > 0.5f) {
            yield return DashToPosition(centerOfArena);
        }

        Vector3 southDirection = new(0f, 0f, -1f);
        Quaternion targetRotation = Quaternion.LookRotation(southDirection);
        _parent.transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);

        int amountOfWaves = IsStronger() ? _waveInfo.AmountOfWavesStronger : _waveInfo.AmountOfWaves;
        float timeBetweenEachWave = IsStronger() ? _waveInfo.TimeBetweenEachWaveStronger : _waveInfo.TimeBetweenEachWave;

        for (int i = 0; i < amountOfWaves; i++) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_waveInfo, 0, _parent.gameObject, _parent.transform.position);
            yield return new WaitForSeconds(timeBetweenEachWave);
        }

        _parent.StartCoroutine(CooldownBetweenAttacks());
    }

    #endregion

    #region RainBulletAttack
    IEnumerator RainBulletAttack() {

        EnemySkillPooling.Instance.RequestInstantiateAttack(_bulletRainInfo, 0, _parent.gameObject);

        yield return new WaitForSeconds(_bulletRainInfo.AttackTime);

        _parent.StartCoroutine(CooldownBetweenAttacks());
    }
    #endregion

    bool IsStronger() {
        return _parent.Health.ReturnCurrentHealth() < _parent.Health.ReturnMaxHealth() / 2;
    }

    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(6);

        Attack();
    }

    #endregion

}

public class BlackBeardFinalFormAttacks {
    public enum FinalFormAttacks { BULLETS, BULLETRAIN, CROSS, WAVE, ANCHOR }
    public FinalFormAttacks Attack;

    public int Priority;
    public float Cooldown;
    public float LastTimeUsed = Mathf.Infinity;

    public bool IsReady => Time.time >= LastTimeUsed + Cooldown;

    public void Use() {
        LastTimeUsed = Time.time;
    }
}
