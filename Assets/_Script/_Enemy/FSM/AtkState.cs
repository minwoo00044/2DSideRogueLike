using System.Collections;
using UnityEngine;

public class AtkState : State, IState
{
    private Coroutine _attackCo; // 실행 중인 코루틴 저장
    private float _animDuration; // SPUM 애니메이션 길이

    public AtkState(StateMachine machine) : base(machine) { }

    public void OnEnter()
    {
        // 1. 애니메이션 재생 및 길이 받아오기 (SPUM 기능 활용)
        // PlayerState enum은 SPUM 코드에 정의된 것을 사용
        machine.owner.spum.PlayAnimation(PlayerState.ATTACK, 0, out _animDuration);

        // 2. 공격 코루틴 시작 (Owner에게 위임하고 핸들 받아오기)
        _attackCo = machine.owner.StartCoroutineHelper(AttackRoutine());

        // 3. 이동 멈춤
        machine.owner.SetMoveDirection(Vector2.zero);
    }

    public void OnUpdate()
    {
        // (옵션) 공격 중에도 타겟을 계속 바라보게 하려면 추가
        /*
        if (machine.target != null)
        {
            float direction = machine.target.position.x - machine.owner.transform.position.x;
            machine.owner.SetMoveDirection(new Vector2(direction, 0).normalized);
        }
        */
    }

    public void OnExit()
    {
        // ★ 중요: 상태를 나갈 때 공격 코루틴이 돌고 있다면 강제 종료
        if (_attackCo != null)
        {
            machine.owner.StopCoroutineHelper(_attackCo);
            _attackCo = null;
        }
    }

    private IEnumerator AttackRoutine()
    {
        // 타격 시점 설정 (예: 애니메이션의 40% 지점에서 타격)
        // EnemyData의 AtkSpeed를 반영하고 싶다면 애니메이션 속도를 조절하거나 딜레이를 계산해야 함
        float hitTime = _animDuration * 0.4f;

        // 1. 타격 전 딜레이
        yield return new WaitForSeconds(hitTime);

        // 2. 데미지 판정 (거리가 여전히 가까운지 체크)
        float distance = Vector2.Distance(machine.owner.transform.position, machine.target.position);

        // 사거리 내에 있고 타겟이 존재하면 데미지
        if (machine.target != null && distance <= machine.owner.AtkRange + 0.5f)
        {
            // Enemy 클래스에 만들어둔 공격 함수 호출
            machine.owner.AttackTarget();
        }

        // 3. 남은 애니메이션 시간 대기 (후딜레이)
        float remainTime = _animDuration - hitTime;
        if (remainTime > 0)
        {
            yield return new WaitForSeconds(remainTime);
        }

        // 4. 공격 후 판단 (쿨타임 적용을 원하면 여기에 추가 대기)

        // 타겟과의 거리를 다시 재서 상태 전환
        float distAfterAttack = Vector2.Distance(machine.owner.transform.position, machine.target.position);

        if (distAfterAttack <= machine.owner.AtkRange)
        {
            // 여전히 사거리 안이라면 -> 다시 공격 (재귀 호출 대신 상태 재진입 활용)
            // OnExit -> OnEnter가 호출되면서 다시 루틴 시작
            machine.ChangeState(EState.Attack);
        }
        else
        {
            // 멀어졌으면 -> 추적 상태로 전환
            machine.ChangeState(EState.Chase);
        }
    }
}