using UnityEngine;
[RequireComponent(typeof(StateMachine))] 

public class Enemy : MonoBehaviour, IDamagable
{
    [Header("Fallback (Inspector)")]
    public float tempHealth = 100f; // 기존 호환용 필드 (기본 체력)
    public float testDamage = 1.0f;

    public PlayerController player { get; set; }
    public SPUM_Prefabs spum {  get; set; }

    // 런타임 사용 현재 체력
    private float currentHealth;
    private StateMachine stateMachine;

    #region EnemyData 복사멤버
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

    [Header("Enemy Stats (카피된 값)")]
    [SerializeField] private string enemyName;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float atkRange;
    [SerializeField] private float atkSpeed;
    [SerializeField] private float maxHp;
    [SerializeField] private float atkDamage;

    // 읽기 전용 프로퍼티
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
        player = FindAnyObjectByType<PlayerController>();
        // Inspector에서 직접 enemyData가 설정되어 있을 수 있으므로 Awake/Start에서 동기화
        ApplyEnemyData();
    }

    private void Start()
    {
        // 복사된 maxHp가 0이면 기존 tempHealth를 사용 (호환성)
        if (maxHp <= 0f)
            maxHp = tempHealth;

        currentHealth = maxHp;
        stateMachine.owner = this;
    }

    private void ApplyEnemyData()
    {
        // enemyData 필드가 Inspector에서 설정되어 있다면 해당 값으로 멤버를 덮어쓴다.
        // (struct 기본값일 때는 tempHealth를 사용하도록 Start에서 처리)
        enemyName = enemyData.EnemyName;
        moveSpeed = enemyData.MoveSpeed;
        atkRange = enemyData.AtkRange;
        atkSpeed = enemyData.AtkSpeed;
        maxHp = enemyData.MaxHp;
        currentHealth = enemyData.MaxHp;
        atkDamage = enemyData.AtkDamage;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage, current health: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //testcode
    private void Die()
    {
        stateMachine.ChangeState(EState.Die);
    }
}