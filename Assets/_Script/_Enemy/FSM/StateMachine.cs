using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public enum EState
{
    Chase,
    Die
}
public class StateMachine : MonoBehaviour
{
    public Enemy owner { get; set; }
    public Transform target { get; set; }
    public IState currentState { get; private set; }

    private Dictionary<EState,IState> eStatePair = new Dictionary<EState,IState>();
    private void Start()
    {
        target = owner.player.transform;
        eStatePair.Add(EState.Chase, new ChaseState(this));
        eStatePair.Add(EState.Die, new DieState(this));
        ChangeState(EState.Chase);
    }
    private void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }
    public void ChangeState(EState state)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = eStatePair[state];
        if (currentState != null)
        {
            currentState.OnEnter();
        }
    }
}
