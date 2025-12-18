// ChaseState.cs

using UnityEngine;

public class ChaseState : State, IState
{
    private float _jumpCooldown = 1.0f; // 점프 쿨타임 (연속 점프 방지)
    private float _lastJumpTime;

    public ChaseState(StateMachine machine) : base(machine) { }

    public void OnEnter()
    {
        machine.owner.spum.PlayAnimation(PlayerState.MOVE, 0);
    }

    public void OnUpdate()
    {
        if (machine.target == null) return;

        Enemy enemy = machine.owner;
        Vector2 targetPos = machine.target.position;
        Vector2 myPos = enemy.transform.position;

        // 1. 공격 사거리 체크 (기존 로직)
        if (Vector2.Distance(myPos, targetPos) <= enemy.AtkRange)
        {
            machine.ChangeState(EState.Attack);
            return;
        }

        // 2. 이동 방향 결정
        Vector2 dir = (targetPos - myPos).normalized;
        enemy.SetMoveDirection(dir);

        // 3. ★ 점프 로직 추가 ★
        // 조건 1: 플레이어가 나보다 일정 높이(1.5f) 이상 위에 있고
        // 조건 2: 적이 바닥에 닿아 있고 (IsGrounded)
        // 조건 3: 점프 쿨타임이 지났다면
        if (Time.time >= _lastJumpTime + _jumpCooldown && enemy.IsGrounded && !enemy.IsObstacleAbove())
        {
            // 1. 플레이어가 뚜렷하게 높은 곳에 있는 경우 (기존 로직)
            if (machine.target.position.y > enemy.transform.position.y + 1.5f)
            {
                // 이때는 플레이어 높이를 목표로 뜀
                enemy.AdaptiveJump(machine.target.position.y);
                _lastJumpTime = Time.time;
            }
            // 2. 플레이어는 같은 높이지만, 가는 길에 '장애물(발판)'이 있는 경우
            else
            {
                // 발판 높이 감지
                float? platformHeight = enemy.GetPlatformHeight();

                // 감지된 발판이 내 발밑보다 높다면 점프
                if (platformHeight.HasValue && platformHeight.Value > enemy.transform.position.y + 0.5f)
                {
                    enemy.AdaptiveJump(platformHeight.Value);
                    _lastJumpTime = Time.time;
                }
            }
        }
    }

    public void OnExit()
    {
        machine.owner.SetMoveDirection(Vector2.zero);
    }
}