using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BlackBeardRunawayState : BlackBeardStates {

    #region Variables

    bool isStuned;
    bool changedState;

    Camps camp;
    BlackBeardRunawaySO _info;

    public float PhaseTimer;

    Coroutine runCoroutine;
    Coroutine phaseCoroutine;

    HealthManager _health;

    EventInstance sound;

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

        if (!_info.JumpSound.IsNull) {
            sound = RuntimeManager.CreateInstance(_info.JumpSound);
            RuntimeManager.AttachInstanceToGameObject(sound, _parent.gameObject);
            sound.start();
        }

        Vector3 landPos = _parent.LandPlace.position;
        landPos.y = GetFloorHeight(landPos); // Corrige a altura do salto

        _parent.transform.DOJump(landPos, _info.JumpPower, 1, _info.JumpDuration).OnComplete(() => {
            Vector3 fromCenter = _parent.transform.position - _parent.CenterOfArena.position;
            Vector3 clampedOffset = fromCenter.normalized * _info.ArenaRadius;
            Vector3 closestPointOnCircle = _parent.CenterOfArena.position + clampedOffset;
            closestPointOnCircle.y = GetFloorHeight(closestPointOnCircle); // Corrige altura

            if (sound.isValid()) {
                sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                sound.release();
            }

            _parent.transform.DOMove(closestPointOnCircle, 0.5f).SetEase(Ease.OutSine).OnComplete(() => {
                SpawnAllies();
                runCoroutine = _parent.StartCoroutine(Runnaway(fromCenter));
                phaseCoroutine = _parent.StartCoroutine(PhaseTimerRoutine());
            });
        });
    }

    void SpawnAllies() {
        int rng = Random.Range(0, _info.ListOfGroups.Count);
        camp.StartCampWithIndex(_info.ListOfGroups[rng].ListOfEnemies.ToArray());
    }

    IEnumerator Runnaway(Vector3 initialOffset) {
        _parent.Health.SetPermissionServerRpc(HealthPermissions.CanTakeDamage, false);

        float angle = Mathf.Atan2(initialOffset.z, initialOffset.x) * Mathf.Rad2Deg;

        if (!_info.RunningSound.IsNull) {
            sound = RuntimeManager.CreateInstance(_info.RunningSound);
            RuntimeManager.AttachInstanceToGameObject(sound, _parent.gameObject);
            sound.start();
        }

        _parent.anim.SetBool("IsWalking", true);
        while (!isStuned && !changedState) {
            angle += _info.BlackBeardSpeed * Time.deltaTime;

            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * _info.ArenaRadius;
            Vector3 targetPos = _parent.CenterOfArena.position + offset;
            targetPos.y = GetFloorHeight(targetPos); // Mantém o boss colado ao chão

            Vector3 currentPos = _parent.transform.position;
            Vector3 nextPos = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * 5f);
            _parent.transform.position = nextPos;
            _parent.transform.forward = (targetPos - currentPos).normalized;

            yield return null;
        }

        _parent.anim.SetBool("IsWalking", false);

        if (sound.isValid()) {
            sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            sound.release();
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
        _parent.anim.SetBool("IsWalking", false);
        _parent.Health.SetPermissionServerRpc(HealthPermissions.CanTakeDamage, true);
        _parent.anim.SetBool("Stun", true);

        if (runCoroutine != null) _parent.StopCoroutine(runCoroutine);
        if (phaseCoroutine != null) _parent.StopCoroutine(phaseCoroutine);

        if (!_info.StunSound.IsNull) {
            sound = RuntimeManager.CreateInstance(_info.StunSound);
            RuntimeManager.AttachInstanceToGameObject(sound, _parent.gameObject);
            sound.start();
        }

        yield return new WaitForSeconds(_info.BlackBeardStunTime);
        _parent.anim.SetBool("IsWalking", false);

        if (sound.isValid()) {
            sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            sound.release();
        }

        if (!changedState) EndPhase();
    }

    void OnDeath() {
        EndPhase();
    }

    void EndPhase() {
        if (changedState) return;

        _parent.anim.SetBool("Stun", false);
        _parent.anim.SetBool("IsWalking", false);

        camp.OnAllEnemiesDead -= Camp_OnAllEnemiesDead;

        changedState = true;

        if (_parent.Lifes > 1) {
            int enemiesAlive = camp.ReturnAliveCount();

            if (enemiesAlive > 0) {
                camp.KillCamp();
                _parent.Health.Heal(_info.AmountOfHealthRecoveredPerEnemy * enemiesAlive, false);
            }

            if (!_info.JumpBackToShipSound.IsNull) {
                sound = RuntimeManager.CreateInstance(_info.JumpBackToShipSound);
                RuntimeManager.AttachInstanceToGameObject(sound, _parent.gameObject);
                sound.start();
            }

            Vector3 shipPos = _parent.ShipPlace.position;
            shipPos.y = GetFloorHeight(shipPos);

            _parent.transform.DOJump(shipPos, _info.JumpPower, 1, _info.JumpDuration).OnComplete(() => {
                _parent.Health.SetPermissionServerRpc(HealthPermissions.CanTakeDamage, true);

                if (sound.isValid()) {
                    sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    sound.release();
                }

                ChangePhase();
            });
        }
        else {
            int enemiesAlive = camp.ReturnAliveCount();
            if (enemiesAlive > 0) camp.KillCamp();
            ChangePhase();
        }
    }

    void ChangePhase() {
        if (changedState) {
            if (_parent.Lifes > 1) _parent.ChangeState(BlackBeardState.SHIP);
            else _parent.ChangeState(BlackBeardState.FINAL);
        }
    }

    float GetFloorHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 15f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor")))
            return hit.point.y + 1f;
        return position.y;
    }
}

