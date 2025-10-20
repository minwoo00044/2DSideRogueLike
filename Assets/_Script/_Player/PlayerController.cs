using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    [Header("컴포넌트 연결")]
    [SerializeField] private SPUM_Prefabs spumAnimationManager;
    [SerializeField] private Slider staminaBar;
    private Rigidbody2D rb;


    [Header("물리 머티리얼 설정")]
    [SerializeField] private PhysicsMaterial2D normalFrictionMaterial;
    [SerializeField] private PhysicsMaterial2D zeroFrictionMaterial;

    [Header("이동 및 점프 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;

    [Header("스태미너 설정")]
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

    public Action<PlayerController> OnDash;
    public Action<PlayerController> OnAttack;

    [Header("장착")]
    public MeleeWeapon weapon;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
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
        // --- 수정된 부분 ---
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0 && !isFacingRight) Flip();
        else if (moveInput < 0 && isFacingRight) Flip();

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
            spumAnimationManager.PlayAnimation(PlayerState.ATTACK, 0);
            OnAttack.Invoke(this);
        }

        UpdateStaminaUI();
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
            Debug.Log("스태미너가 부족합니다!");
            return;
        }

        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        Vector2 dashDirection = new Vector2(hInput, vInput);

        if (dashDirection == Vector2.zero)
        {
            if (isGrounded) 
            {
                // Y축 속도를 초기화하여 점프 높이를 일정하게 만듦
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * dashForce, ForceMode2D.Impulse);

                // 점프 애니메이션 호출 로직
                // spumAnimationManager.PlayAnimation(PlayerState.OTHER, 1); // 예시
            }
            return; // 점프 처리 후 함수 종료
        }
        if (vInput < 0 && hInput == 0 && isGrounded) return;

        currentStamina -= 1;
        StartCoroutine(DashCoroutine(dashDirection.normalized));
    }

    private IEnumerator DashCoroutine(Vector2 dashDirection)
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
            // 1. 풀에서 잔상 하나를 가져온다
            AfterimageSprite afterimage = AfterimagePool.instance.GetFromPool();
            if (afterimage != null)
            {
                // 2. 현재 플레이어의 모습으로 세팅한다
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


}