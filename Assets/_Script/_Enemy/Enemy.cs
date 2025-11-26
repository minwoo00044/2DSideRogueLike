using System.Collections;
using UnityEngine;
[RequireComponent(typeof(StateMachine))] 

public class Enemy : MonoBehaviour, IDamagable
{
    [Header("Fallback (Inspector)")]
    public float tempHealth = 100f; 
    public float testDamage = 1.0f;

    public PlayerController player { get; set; }
    public SPUM_Prefabs spum {  get; set; }

    // ��Ÿ�� ��� ���� ü��
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

    // �б� ���� ������Ƽ
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
}