using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public Enemy owner { get; set; }
    public Transform target { get; set; }
    public IState currentState { get; private set; }
    private void Start()
    {
        target = owner.player.transform;
        ChangeState(new ChaseState(this));
    }
    private void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }
    public void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = newState;
        if (currentState != null)
        {
            currentState.OnEnter();
        }
    }
}
