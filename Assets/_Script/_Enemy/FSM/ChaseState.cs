
using UnityEngine;

public class ChaseState : State, IState
{
    private Transform target;
    public ChaseState(StateMachine machine) : base(machine)
    {
    }

    public void OnEnter()
    {
        target = machine.target;

        machine.owner.spum.PlayAnimation(PlayerState.MOVE, 0);
    }

    public void OnUpdate()
    {

        // 플레이어가 사거리에 들어오면
        //if (Vector2.Distance(machine.transform.position, target.position) <= machine.owner.AtkRange)
        //{
        //    //machine.ChangeState(new AttackState()); // Attack 상태로 전환
        //    return;
        //}

        // 플레이어 추적 로직...
        Vector2 direction = (target.position - machine.transform.position).normalized;
        machine.owner.SetMoveDirection(direction);
    }

    public void OnExit()
    {
        // monster.anim.SetBool("IsChasing", false);
        //monster.rb.velocity = Vector2.zero; // 멈춤
    }
}