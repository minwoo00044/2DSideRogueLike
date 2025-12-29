using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamagable
{

    [Header("컴포넌트 설정")]
    [SerializeField] private SPUM_Prefabs spumAnimationManager;
    [SerializeField] private Slider staminaBar;
    private Rigidbody2D rb;


    [Header("물리 재질 설정")]
    [SerializeField] private PhysicsMaterial2D normalFrictionMaterial;
    [SerializeField] private PhysicsMaterial2D zeroFrictionMaterial;

    [Header("이동 및 대시 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;

    [Header("스태미나 설정")]
    [SerializeField] private float maxStamina = 3f;
    [SerializeField] private float staminaRegenRate = 2f;
    [SerializeField] private float staminaRegenDelay = 1f;
    private float currentStamina;
    private float staminaRegenTimer;

    [Header("지면 체크 설정")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    private bool isFacingRight = false;
    private bool isDashing = false;
    private bool isAttacking = false;

    public Action<PlayerController> OnDash;
    public Action<PlayerController> OnAttack;

    //[Header("아이템")]
    private Dictionary<ItemType, Item> items = new Dictionary<ItemType, Item>();
    private PlayerDamageCalculator damageCalculator = new PlayerDamageCalculator();
    public Weapon w;
    public MeleeWeaponData d;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        items.Clear();
        items.Add(ItemType.Weapons, new Weapon(this));
        // 초기화 코드
        // OnAttack += (this)=> Debug.Log(items[ItemType.Weapons].Data.ItemName) ;

        // 이벤트 리스너 설정
    }

    private void Start()
    {
        currentStamina = maxStamina;
        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = currentStamina;
        }
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
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

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

        if  (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
            {
                StartCoroutine(AttackRoutine());
            }
        }

        UpdateStaminaUI();
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
        if (staminaRegenTimer > 0)
        {
            staminaRegenTimer -= Time.deltaTime;
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += (1 / staminaRegenRate) * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
    }

    private void HandleDash()
    {
        if (currentStamina < 1)
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
                rb.AddForce(Vector2.up * dashForce, ForceMode2D.Impulse);

                // 대시 애니메이션 호출 예시
                // spumAnimationManager.PlayAnimation(PlayerState.OTHER, 1); // 점프
            }
            return; // 로직 처리 후 리턴
        }
        if (vInput < 0 && hInput == 0 && isGrounded) return;

        currentStamina -= 1;
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
            dashDirection.x * dashForce,
            dashDirection.y * dashForce
        );

        rb.AddForce(finalDashForce, ForceMode2D.Impulse);

        float afterimageTimer = 0f;
        while (afterimageTimer < dashDuration)
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

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        if (wasGrounded) rb.sharedMaterial = normalFrictionMaterial;

        isDashing = false;
        staminaRegenTimer = staminaRegenDelay;
    }

    private void UpdateStaminaUI()
    {
        if (staminaBar != null)
        {
            staminaBar.value = currentStamina;
        }
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
        if(plus != 0)
            damageCalculator.AddModifier("Buff_Flat", plus, false);
        if(multiple != 0)
            damageCalculator.AddModifier("Buff_Percent", multiple, true);
    }
    private void OnDrawGizmos()
    {
        if ( !items.ContainsKey(ItemType.Weapons) || items[ItemType.Weapons] == null ) return;
        Weapon weapon = items[ItemType.Weapons] as Weapon;
        weapon.DrawGizmos(transform);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Player took {damage} damage.");
    }
}