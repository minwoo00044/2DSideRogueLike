
using UnityEngine;

public class DieState : State, IState
{
    public DieState(StateMachine machine) : base(machine)
    {
    }

    public void OnEnter()
    {
        machine.owner.spum.PlayAnimation(PlayerState.DEATH, 0);
        machine.gameObject.SetActive(false);
    }

    public void OnUpdate()
    {
        // 몬스터 체력이 0이면
        //if (machine.hp <= 0)
        //{
        //    monster.ChangeState(new DieState()); // Die 상태로 전환
        //    return;
        //}

        //// 플레이어가 사거리에 들어오면
        //if (Vector2.Distance(machine.transform.position, target.position) <= machine.attackRange)
        //{
        //    monster.ChangeState(new AttackState()); // Attack 상태로 전환
        //    return;
        //}

        //// 플레이어 추적 로직...
        //Vector2 direction = (target.position - monster.transform.position).normalized;
        //monster.rb.velocity = new Vector2(direction.x * monster.moveSpeed, monster.rb.velocity.y);
    }

    public void OnExit()
    {
        // monster.anim.SetBool("IsChasing", false);
        //monster.rb.velocity = Vector2.zero; // 멈춤
    }
}