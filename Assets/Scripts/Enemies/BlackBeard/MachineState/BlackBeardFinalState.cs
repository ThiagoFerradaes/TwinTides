using UnityEngine;

public class BlackBeardFinalState : BlackBeardStates {

    public override void StartState(BlackBeardMachineState parent) {
        base.StartState(parent);

        _parent.Lifes = 0;
    }

}
