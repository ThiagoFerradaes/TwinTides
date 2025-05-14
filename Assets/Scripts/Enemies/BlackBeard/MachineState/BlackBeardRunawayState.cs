using UnityEngine;

public class BlackBeardRunawayState : BlackBeardStates {

    public override void StartState(BlackBeardMachineState parent) {
        base.StartState(parent);
        Debug.Log("RunnawayState");
    }

 
}
