using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ChangeUIManager : MonoBehaviour
{
    [SerializeField] private Slider hpSliderUI;
    [SerializeField] private Slider staminaSliderUI;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI staminaText;
    private PlayerStat playerStat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStat = FindAnyObjectByType<PlayerStat>();
        if (playerStat != null)
        {
            playerStat.OnHpChanged += UpdateHpUI;
            playerStat.OnStaminaChanged += UpdateStaminaUI;

            // 초기 UI 설정
            UpdateHpUI(playerStat.CurrentHP, playerStat.MaxHP);
            UpdateStaminaUI(playerStat.CurrentStamina, playerStat.MaxStamina);
        }
    }

    private void UpdateHpUI(float currentHp, float maxHp)
    {
        if (hpSliderUI != null)
        {
            hpSliderUI.value = currentHp / maxHp;
        }
        if (hpText != null)
        {
            hpText.text = $"{currentHp}/{maxHp}";
        }
    }

    private void UpdateStaminaUI(float currentStamina, float maxStamina)
    {
        if (staminaSliderUI != null)
        {
            staminaSliderUI.value = currentStamina / maxStamina;
        }
        if (staminaText != null)
        {
            staminaText.text = $"{(int)currentStamina}/{(int)maxStamina}";
        }
    }

    private void OnDestroy()
    {
        if (playerStat != null)
        {
            playerStat.OnHpChanged -= UpdateHpUI;
            playerStat.OnStaminaChanged -= UpdateStaminaUI;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
