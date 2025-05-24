using UnityEngine;

public abstract class BlackBeardStates 
{
    protected BlackBeardMachineState _parent;
    public virtual void StartState(BlackBeardMachineState parent) {
        _parent = parent;
    }
}
