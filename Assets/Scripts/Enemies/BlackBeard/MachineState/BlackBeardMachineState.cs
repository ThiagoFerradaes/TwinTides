using System.Collections.Generic;
using UnityEngine;

public class BlackBeardMachineState : MonoBehaviour
{
    BlackBeardStates _currentState;
    BlackBeardShipState _shipState = new();
    BlackBeardRunawayState _runawayState = new();
    BlackBeardFinalState _finalState = new();
    public List<BlackBeardSO> ListOfAttacks = new();
    void Start()
    {
        _currentState = _shipState;
        _currentState.StartState(this);
    }

    void Update()
    {

    }
}
