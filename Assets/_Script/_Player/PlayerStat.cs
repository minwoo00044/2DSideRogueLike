using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStat : MonoBehaviour
{

    [Header("이동 및 대시 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashForce = 5f;
    [SerializeField] private float dashDuration = 0.2f;

    [Header("스태미나 설정")]
    [SerializeField] private float maxStamina = 3f;
    [SerializeField] private float staminaRegenRate = 0.5f;
    [SerializeField] private float staminaRegenDelay = 1f;

    [Header("체력 설정")]
    [SerializeField] private float maxHp = 100f;

    [Header("이벤트")]
    public Action<float, float> OnHpChanged;
    public Action OnDeath;
    public Action<float, float> OnStaminaChanged;

    // 런타임 변수
    private float _currentStamina;
    public float CurrentStamina
    {
        get => _currentStamina;
        set
        {
            _currentStamina = value;
            OnStaminaChanged?.Invoke(_currentStamina, maxStamina);
        }
    }
    public float StaminaRegenTimer { get; set; }
    public float CurrentHP { get; set; }

    // 외부 접근용 프로퍼티
    public float MoveSpeed => moveSpeed;
    public float DashForce => dashForce;
    public float DashDuration => dashDuration;
    public float MaxStamina => maxStamina;
    public float StaminaRegenRate => staminaRegenRate;
    public float StaminaRegenDelay => staminaRegenDelay;
    public float MaxHP => maxHp;

    private void Awake()
    {
        CurrentStamina = maxStamina;
        CurrentHP = maxHp;
    }
    private void Start()
    {

    }
    public void ChangeStamina(float stamina)
    {
        CurrentStamina += stamina;
        if (CurrentStamina > maxStamina)
        {
            CurrentStamina = maxStamina;
        }
    }
    public void ChangeHp(float damage)
    {
        if (CurrentHP <= 0) return; // 이미 죽었다면 무시

        CurrentHP -= damage;

        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            OnHpChanged?.Invoke(CurrentHP, maxHp);
            OnDeath?.Invoke(); // 사망 이벤트 발생
        }
        else
        {
            OnHpChanged?.Invoke(CurrentHP, maxHp);
        }
    }
}
