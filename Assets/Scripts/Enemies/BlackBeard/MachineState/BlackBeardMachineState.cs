using System.Collections.Generic;
using UnityEngine;

public enum BlackBeardState { SHIP, RUNNAWAY, FINAL }
public class BlackBeardMachineState : MonoBehaviour
{
    
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

    public int Lifes = 1;
    void Start()
    {
        _currentState = _finalState;
        _currentState.StartState(this);
    }

    public void ChangeState(BlackBeardState state) {
        _currentState = state switch {
            BlackBeardState.SHIP => _shipState,
            BlackBeardState.RUNNAWAY => _runawayState,
            BlackBeardState.FINAL => _finalState,
            _ => _shipState
        };

        _currentState.StartState(this);
    }
}
