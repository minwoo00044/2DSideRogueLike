using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerStat))]
public class PlayerController : MonoBehaviour, IDamagable
{

    [Header("컴포넌트 설정")]
    [SerializeField] private SPUM_Prefabs spumAnimationManager;
    private Rigidbody2D rb;
    private PlayerStat playerStat;


    [Header("물리 재질 설정")]
    [SerializeField] private PhysicsMaterial2D normalFrictionMaterial;
    [SerializeField] private PhysicsMaterial2D zeroFrictionMaterial;

    [Header("지면 체크 설정")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    private bool isFacingRight = false;
    private bool isDashing = false;
    private bool isAttacking = false;
    [Header("이벤트")]
    public Action<PlayerController> OnDash;
    public Action<PlayerController, float> OnDamaged;
    public Action<PlayerController> OnAttack;

    //[Header("아이템")]
    private Dictionary<ItemType, Item> items = new Dictionary<ItemType, Item>();
    private PlayerDamageCalculator damageCalculator = new PlayerDamageCalculator();
    public Weapon w;
    public MeleeWeaponData d;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStat = GetComponent<PlayerStat>();
        items.Clear();
        items.Add(ItemType.Weapons, new Weapon(this));
        // 초기화 코드
        // OnAttack += (this)=> Debug.Log(items[ItemType.Weapons].Data.ItemName) ;

        // 이벤트 리스너 설정
    }

    private void Start()
    {
        if (spumAnimationManager != null)
        {
            spumAnimationManager.OverrideControllerInit();
            spumAnimationManager.PopulateAnimationLists();
        }
        if (rb != null && normalFrictionMaterial != null)
        {
            rb.sharedMaterial = normalFrictionMaterial;
        }

    }

    void Update()
    {
        if (isDashing) return;

        HandleStaminaRegen();
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        float moveInput = Input.GetAxis("Horizontal");
        // --- 이동 처리 ---
        rb.linearVelocity = new Vector2(moveInput * playerStat.MoveSpeed, rb.linearVelocity.y);

        if (moveInput > 0 && !isFacingRight) Flip();
        else if (moveInput < 0 && isFacingRight) Flip();
        if (moveInput == 0)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        if (Mathf.Abs(moveInput) > 0.1f)
            spumAnimationManager.PlayAnimation(PlayerState.MOVE, 0);
        else
            spumAnimationManager.PlayAnimation(PlayerState.IDLE, 0);

        if (Input.GetButtonDown("Jump"))
        {
            HandleDash();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
            {
                StartCoroutine(AttackRoutine());
            }
        }
    }
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        float animDuration;
        spumAnimationManager.PlayAnimation(PlayerState.ATTACK, 0, out animDuration);

        float hitTiming = animDuration * 0.3f;
        yield return new WaitForSeconds(hitTiming);

        OnAttack?.Invoke(this);

        yield return new WaitForSeconds(animDuration - hitTiming);
        isAttacking = false;
        spumAnimationManager.PlayAnimation(PlayerState.IDLE, 0);
    }
    private void HandleStaminaRegen()
    {
        if (playerStat.StaminaRegenTimer > 0)
        {
            playerStat.StaminaRegenTimer -= Time.deltaTime;
        }
        else if (playerStat.CurrentStamina < playerStat.MaxStamina)
        {
            playerStat.CurrentStamina += playerStat.StaminaRegenRate * Time.deltaTime;
            playerStat.CurrentStamina = Mathf.Min(playerStat.CurrentStamina, playerStat.MaxStamina);
        }
    }

    private void HandleDash()
    {
        if (playerStat.CurrentStamina < 1)
        {
            Debug.Log("스태미나가 부족합니다!");
            return;
        }

        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        Vector2 dashDirection = new Vector2(hInput, vInput);

        if (dashDirection == Vector2.zero)
        {
            if (isGrounded)
            {
                // Y축 속도를 초기화하여 일정 높이 점프 보장
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * playerStat.DashForce, ForceMode2D.Impulse);

                // 대시 애니메이션 호출 예시
                // spumAnimationManager.PlayAnimation(PlayerState.OTHER, 1); // 점프
            }
            return; // 로직 처리 후 리턴
        }
        if (vInput < 0 && hInput == 0 && isGrounded) return;

        playerStat.ChangeStamina(-1);
        StartCoroutine(DashCoroutine(dashDirection.normalized));
    }

    private System.Collections.IEnumerator DashCoroutine(Vector2 dashDirection)
    {
        bool wasGrounded = isGrounded;
        isDashing = true;

        if (wasGrounded) rb.sharedMaterial = zeroFrictionMaterial;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        Vector2 finalDashForce = new Vector2(
            dashDirection.x * playerStat.DashForce,
            dashDirection.y * playerStat.DashForce
        );

        rb.AddForce(finalDashForce, ForceMode2D.Impulse);

        float afterimageTimer = 0f;
        while (afterimageTimer < playerStat.DashDuration)
        {
            // 1. 풀에서 잔상 가져오기
            AfterimageSprite afterimage = AfterimagePool.instance.GetFromPool();
            if (afterimage != null)
            {
                // 2. 잔상 설정 (현재 위치 등)
                afterimage.Setup(transform);
            }

            afterimageTimer += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(playerStat.DashDuration);

        rb.gravityScale = originalGravity;
        if (wasGrounded) rb.sharedMaterial = normalFrictionMaterial;

        isDashing = false;
        playerStat.StaminaRegenTimer = playerStat.StaminaRegenDelay;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void ChangeItem(ItemData data)
    {
        items[data.ItemType].Equip(data);
        d = data as MeleeWeaponData;
    }
    public void AddBuff(float plus, float multiple)
    {
        if (plus != 0)
            damageCalculator.AddModifier("Buff_Flat", plus, false);
        if (multiple != 0)
            damageCalculator.AddModifier("Buff_Percent", multiple, true);
    }
    private void OnDrawGizmos()
    {
        if (!items.ContainsKey(ItemType.Weapons) || items[ItemType.Weapons] == null) return;
        Weapon weapon = items[ItemType.Weapons] as Weapon;
        weapon.DrawGizmos(transform);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Player took {damage} damage.");
        spumAnimationManager.PlayAnimation(PlayerState.DAMAGED, 0);
        OnDamaged?.Invoke(this, damage);
        playerStat.ChangeHp(damage);
    }
}