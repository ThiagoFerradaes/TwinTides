using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BlackBeardState { SHIP, RUNNAWAY, FINAL }
public class BlackBeardMachineState : NetworkBehaviour {

    BlackBeardStates _currentState;
    BlackBeardShipState _shipState = new();
    BlackBeardRunawayState _runawayState = new();
    BlackBeardFinalState _finalState = new();

    public List<BlackBeardSO> ListOfAttacks = new();
    public Transform CenterOfArena;
    public Transform[] CannonsPosition;
    public Transform Ship;
    public Transform ShipPlace, LandPlace;
    public Camps ShipCamp;

    public HealthManager Health;

    public int Lifes = 2;

    public static event Action OnFinal;
    public event Action OnChangedPhase;
    public static event Action OnDeath;
    public void StartFight() {
        Debug.Log("BlackBeard Started");
        StartCoroutine(WaitToStart());
    }

    IEnumerator WaitToStart() {
        yield return new WaitForSeconds(0.5f);
        _currentState = _shipState;
        _currentState.StartState(this);
    }

    public void ChangeState(BlackBeardState state) {
        _currentState = state switch {
            BlackBeardState.SHIP => _shipState,
            BlackBeardState.RUNNAWAY => _runawayState,
            BlackBeardState.FINAL => _finalState,
            _ => _shipState
        };

        if (state == BlackBeardState.FINAL) OnFinal?.Invoke();

        OnChangedPhase?.Invoke();

        _currentState.StartState(this);
    }

    public void FinalFormChoosAttack(List<BlackBeardFinalFormAttacks> attacks) {
        if (!IsServer) return;

        BlackBeardFinalFormAttacks attack = ChooseAttack(attacks);

        FinalFormAttackRpc(attack.Attack);
    }

    BlackBeardFinalFormAttacks ChooseAttack(List<BlackBeardFinalFormAttacks> attacks) {
        for (int priority = 1; priority <= 3; priority++) {
            var available = attacks.Where(a => a.Priority == priority && a.IsReady).ToList();

            if (available.Count > 0) {
                return available[Random.Range(0, available.Count)];
            }
        }

        return null;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void FinalFormAttackRpc(BlackBeardFinalFormAttacks.FinalFormAttacks attack) {
        if (_currentState is BlackBeardFinalState finalState) {
            finalState.Attack(attack);
        }
    }

    public void Death() {
        if (_currentState != _finalState || Lifes > 0) return;

        StopAllCoroutines();

        OnDeath?.Invoke();

        gameObject.SetActive(false);
    }
}
