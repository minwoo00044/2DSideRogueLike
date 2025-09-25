using UnityEngine;

public class _Tester : MonoBehaviour
{
    public PlayerController controller;
    public MeleeWeapon MeleeTestWeapon;
    public MeleeWeaponData MeleeTestWeaponData;

    void Start()
    {
        // ScriptableObject가 아닌 일반 클래스라면 new로 생성 가능합니다.
        MeleeTestWeaponData = new MeleeWeaponData("TestMelee", "Weapons", 0, 10, new Vector2(1f, 0.5f), new Vector2(1.5f, 1f));
        MeleeTestWeapon = new MeleeWeapon();
        MeleeTestWeapon.Init(MeleeTestWeaponData);

        // controller가 할당되었을 때만 무기를 장착시킵니다.
        if (controller != null)
        {
            MeleeTestWeapon.Equip(controller);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        if (controller == null || MeleeTestWeaponData == null) return;

        Gizmos.color = Color.red;

        Vector2 currentPosition = controller.transform.position;

        // 1. 캐릭터의 현재 localScale.x 값을 이용해 바라보는 방향을 구합니다. (오른쪽: 1, 왼쪽: -1)
        float facingDirection = -Mathf.Sign(controller.transform.localScale.x);

        // 2. 방향을 적용한 새로운 공격 오프셋을 계산합니다.
        // AttackOffset.x에 facingDirection을 곱해줘서 캐릭터가 왼쪽을 보면 x 오프셋이 음수가 되도록 합니다.
        Vector2 offset = new Vector2(MeleeTestWeaponData.AttackOffset.x * facingDirection, MeleeTestWeaponData.AttackOffset.y);

        // 3. 최종 공격 중심 위치를 계산합니다.
        Vector2 attackCenter = currentPosition + offset;

        Gizmos.DrawWireCube(attackCenter, MeleeTestWeaponData.BoxSize);
    }
}