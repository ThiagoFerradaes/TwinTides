using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BlackBeardRunawayState : BlackBeardStates {

    #region Variables

    // Booleanas
    bool isStuned;
    bool changedState;

    // Camp
    Camps camp;

    // SO
    BlackBeardRunawaySO _info;

    // float
    public float PhaseTimer;

    // Corrotinas
    Coroutine runCoroutine;
    Coroutine phaseCoroutine;

    // Health
    HealthManager _health;

    #endregion
    public override void StartState(BlackBeardMachineState parent) {
        base.StartState(parent);

        RestartState();

        LeaveShip();
    }

    void RestartState() {

        if (camp == null) camp = _parent.ShipCamp;

        if (_info == null) _info = _parent.ListOfAttacks[1] as BlackBeardRunawaySO;

        if (_health == null) _health = _parent.Health;

        isStuned = false;
        changedState = false;

        PhaseTimer = _info.PhaseDuration;

        camp.OnAllEnemiesDead -= Camp_OnAllEnemiesDead;
        camp.OnAllEnemiesDead += Camp_OnAllEnemiesDead;

        _health.OnDeath -= OnDeath;
        _health.OnDeath += OnDeath;
    }



    private void Camp_OnAllEnemiesDead() {
        if (!changedState && !isStuned) {
            _parent.StartCoroutine(StunTimer());
        }
    }

    void LeaveShip() {

        _parent.transform.DOKill();

        _parent.transform.DOJump(_parent.LandPlace.position, _info.JumpPower, 1, _info.JumpDuration).OnComplete(() => {
            Vector3 fromCenter = _parent.transform.position - _parent.CenterOfArena.position;
            Vector3 clampedOffset = fromCenter.normalized * _info.ArenaRadius;
            Vector3 closestPointOnCircle = _parent.CenterOfArena.position + clampedOffset;

            _parent.transform.DOMove(closestPointOnCircle, 0.5f).SetEase(Ease.OutSine).OnComplete(() => {
                SpawnAllies();
                runCoroutine = _parent.StartCoroutine(Runnaway(fromCenter));
                phaseCoroutine = _parent.StartCoroutine(PhaseTimerRoutine());
            });
        }
      );
    }

    void SpawnAllies() {
        int rng = Random.Range(0, _info.ListOfGroups.Count);

        camp.StartCampWithIndex(_info.ListOfGroups[rng].ListOfEnemies);
    }

    IEnumerator Runnaway(Vector3 initialOffset) {
        _parent.Health.SetPermissionServerRpc(HealthPermissions.CanTakeDamage, false);

        float angle = Mathf.Atan2(initialOffset.z, initialOffset.x) * Mathf.Rad2Deg;

        while (!isStuned && !changedState) {
            angle += _info.BlackBeardSpeed * Time.deltaTime;

            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * _info.ArenaRadius;
            Vector3 targetPos = _parent.CenterOfArena.position + offset;

            _parent.transform.position = Vector3.Lerp(_parent.transform.position, targetPos, Time.deltaTime * 5f);
            _parent.transform.forward = (targetPos - _parent.transform.position).normalized;

            yield return null;
        }
    }

    IEnumerator PhaseTimerRoutine() {
        while (PhaseTimer > 0 && !changedState) {
            PhaseTimer -= Time.deltaTime;
            yield return null;
        }

        if (!isStuned && !changedState) EndPhase();
    }

    IEnumerator StunTimer() {
        if (isStuned || changedState) yield break;

        isStuned = true;
        _parent.Health.SetPermissionServerRpc(HealthPermissions.CanTakeDamage, true);

        if (runCoroutine != null) _parent.StopCoroutine(runCoroutine);
        if (phaseCoroutine != null) _parent.StopCoroutine(phaseCoroutine);

        yield return new WaitForSeconds(_info.BlackBeardStunTime);

        if (!changedState) EndPhase();
    }

    void OnDeath() {
        _parent.Lifes = 1;
        EndPhase();
    }
    void EndPhase() {
        if (changedState) return;

        camp.OnAllEnemiesDead -= Camp_OnAllEnemiesDead;

        changedState = true;

        if (_parent.Lifes > 1) {
            int enemiesAlive = camp.ReturnAliveCount();

            if (enemiesAlive > 0) {
                camp.KillCamp();

                _parent.Health.Heal(_info.AmountOfHealthRecoveredPerEnemy * enemiesAlive, false);
            }

            _parent.transform.DOJump(_parent.ShipPlace.position, _info.JumpPower, 1, _info.JumpDuration).OnComplete(() => {
                _parent.Health.SetPermissionServerRpc(HealthPermissions.CanTakeDamage, true);

                ChangePhase();
            });
        }
        else {
            int enemiesAlive = camp.ReturnAliveCount();

            if (enemiesAlive > 0) {
                camp.KillCamp();
            }

            ChangePhase();
        }
    }

    void ChangePhase() {
        if (changedState) {
            if (_parent.Lifes > 1) _parent.ChangeState(BlackBeardState.SHIP);
            else _parent.ChangeState(BlackBeardState.FINAL);
        }
    }
}
