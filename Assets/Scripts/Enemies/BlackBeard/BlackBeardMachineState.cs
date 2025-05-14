using UnityEngine;

public class BlackBeardMachineState : MonoBehaviour
{
    BlackBeardStates _currentState;
    BlackBeardShipState _shipState = new();
    BlackBeardRunawayState _runawayState = new();
    BlackBeardFinalState _finalState = new();

    void Start()
    {
        _currentState = _shipState;
        _currentState.StartState();
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.UpdateState();
    }
}
