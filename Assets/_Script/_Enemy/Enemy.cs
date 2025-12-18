using System.Collections;
using UnityEngine;
[RequireComponent(typeof(StateMachine))] 

public class Enemy : MonoBehaviour, IDamagable
{
    [Header("Fallback (Inspector)")]
    public float tempHealth = 100f; 
    public float testDamage = 1.0f;
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;       // 점프 힘
    [SerializeField] private Transform groundCheck;      // 발바닥 위치 (빈 오브젝트)
    [SerializeField] private float groundCheckRadius = 0.2f; // 바닥 체크 범위
    [Header("Adaptive Jump")]
    [SerializeField] private float checkDistance = 1.0f; // 전방 감지 거리
    [SerializeField] private LayerMask groundLayer;      // 땅 레이어
    public PlayerController player { get; set; }
    public SPUM_Prefabs spum {  get; set; }
    public bool IsGrounded
    {
        get
        {
            // 발바닥 위치에 원을 그려서 땅 레이어가 걸리는지 확인
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
    }

    [SerializeField]
    private float currentHealth;
    private StateMachine stateMachine;
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    [SerializeField]
    private Vector2 _moveInput;
    #region EnemyData
    [SerializeField]
    private EnemyData enemyData;
    public EnemyData EnemyData
    {
        get { return enemyData; }
        set
        {
            enemyData = value;
            ApplyEnemyData();
        }
    }

    [Header("Enemy Stats")]
    [SerializeField] private string enemyName;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float atkRange;
    [SerializeField] private float atkSpeed;
    [SerializeField] private float maxHp;
    [SerializeField] private float atkDamage;

    public string EnemyName => enemyName;
    public float MoveSpeed => moveSpeed;
    public float AtkRange => atkRange;
    public float AtkSpeed => atkSpeed;
    public float MaxHp => maxHp;
    public float CurrentHealth => currentHealth;
    public float AtkDamage => atkDamage;
    #endregion
    [ContextMenu("Damge")]
    public void TestDamage()
    {
        TakeDamage(testDamage);
    }
    private void Awake()
    {
        stateMachine = GetComponent<StateMachine>();
        spum = GetComponent<SPUM_Prefabs>();
        if (spum != null)
        {
            spum.OverrideControllerInit();
            spum.PopulateAnimationLists();
        }
        player = FindAnyObjectByType<PlayerController>();
        stateMachine.owner = this;
        stateMachine.target = player.transform;
        ApplyEnemyData();
    }

    private void Start()
    {

        if (maxHp <= 0f)
            maxHp = tempHealth;

        currentHealth = maxHp;
    }
    private void FixedUpdate()
    {
        // 최종적으로 들어가는 값이 얼마인지 확인

        rb.linearVelocity = new Vector2(_moveInput.x * moveSpeed, rb.linearVelocity.y);
    }
    private void ApplyEnemyData()
    {

        enemyName = enemyData.EnemyName;
        moveSpeed = enemyData.MoveSpeed;
        atkRange = enemyData.AtkRange;
        atkSpeed = enemyData.AtkSpeed;
        maxHp = enemyData.MaxHp;
        currentHealth = enemyData.MaxHp;
        atkDamage = enemyData.AtkDamage;
    }
    public void SetMoveDirection(Vector2 direction)
    {
        _moveInput = direction;
        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (direction.x > 0 ? -1 : 1);
            transform.localScale = scale;
        }
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage, current health: {currentHealth}");
        if (currentHealth <= 0)
        {
            stateMachine.ChangeState(EState.Die);
        }
    }

    //testcode
    public IEnumerator DelayDie(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
    public Coroutine StartCoroutineHelper(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    // [추가] 상태가 바뀔 때 코루틴을 확실히 끊어주는 헬퍼
    public void StopCoroutineHelper(Coroutine routine)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
    }

    // [추가] 실제 데미지 판정 로직 (State에서 호출)
    public void AttackTarget()
    {
        IDamagable target = player as IDamagable;
        if (target != null)
        {
            // EnemyData의 공격력을 사용
            target.TakeDamage(atkDamage);
        }
    }
    public float? GetPlatformHeight()
    {
        Vector2 frontOrigin = (Vector2)transform.position + new Vector2(transform.localScale.x * 0.5f, 0);
        Vector2 rayDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // 1. 앞에 벽이 있는지 확인
        RaycastHit2D wallHit = Physics2D.Raycast(frontOrigin, rayDir, checkDistance, groundLayer);

        // 벽이 없다면 점프할 필요 없음 (혹은 낭떠러지 로직 별도)
        if (!wallHit) return null;

        // 2. 벽이 있다면, 그 벽의 '꼭대기'가 어디인지 찾기 위해 위에서 아래로 레이를 쏨
        // 벽보다 조금 더 앞, 그리고 높은 곳에서 아래로 쏨
        Vector2 ceilingOrigin = frontOrigin + (rayDir * checkDistance) + (Vector2.up * 3.0f); // 3.0f는 최대 점프 높이 가정
        RaycastHit2D topHit = Physics2D.Raycast(ceilingOrigin, Vector2.down, 4.0f, groundLayer);

        if (topHit)
        {
            // 발판의 윗면 Y좌표 반환
            return topHit.point.y;
        }

        return null;
    }

    public void AdaptiveJump(float targetY)
    {
        // 목표 높이 차이 계산
        // targetY - 현재Y = 올라가야 할 높이
        // + 0.5f 여유값 (딱 맞춰 뛰면 모서리에 걸릴 수 있음)
        float heightToReach = (targetY - transform.position.y) + 0.5f;

        // 높이가 0보다 작으면(내리막) 점프 불필요하거나 약하게 점프
        if (heightToReach <= 0) heightToReach = 0.5f; // 최소 점프

        // 공식: v = sqrt(2 * g * h)
        // rb.gravityScale을 곱해줘야 실제 적용되는 중력 반영
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float jumpVelocity = Mathf.Sqrt(2 * gravity * heightToReach);

        // 기존 X 속도는 유지하고 Y 속도만 덮어씌움
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);

        // 애니메이션
        // spum.PlayAnimation(PlayerState.JUMP, 0); 
    }
    public void Jump()
    {
        // Y축으로 힘을 가함 (ForceMode2D.Impulse는 순간적인 힘)
        // 기존 X축 속도는 유지해야 자연스럽게 대각선 점프가 됨
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // 점프 전 Y속도 초기화 (일정 높이 보장)
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // 점프 애니메이션 재생 (SPUM에 점프 모션이 있다면 사용, 없다면 MOVE 유지)
        // spum.PlayAnimation(PlayerState.OTHER, 0); // 예시: OTHER에 점프가 있다면
    }

    public bool IsObstacleAbove()
    {
        // 머리 위 장애물 체크 (거리 1.5f 정도)
        return Physics2D.Raycast(transform.position, Vector2.up, 1.5f, groundLayer);
    }

    // 에디터에서 바닥 체크 범위 눈으로 보기 (디버깅용)
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}