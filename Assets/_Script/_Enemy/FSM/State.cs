using UnityEngine;
public abstract class State
{
    protected StateMachine machine;
    public State(StateMachine machine)
    {
        this.machine = machine;
    }
}