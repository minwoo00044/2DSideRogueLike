using UnityEngine;

public class _Tester : MonoBehaviour
{
    public PlayerController controller;
    public MeleeWeapon MeleeTestWeapon;
    public MeleeWeaponData MeleeTestWeaponData;

    void Start()
    {
        // ScriptableObject�� �ƴ� �Ϲ� Ŭ������� new�� ���� �����մϴ�.
        MeleeTestWeaponData = new MeleeWeaponData("TestMelee", "Weapons", 0, 10, new Vector2(1f, 0.5f), new Vector2(1.5f, 1f));
        MeleeTestWeapon = new MeleeWeapon();
        MeleeTestWeapon.Init(MeleeTestWeaponData);

        // controller�� �Ҵ�Ǿ��� ���� ���⸦ ������ŵ�ϴ�.
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

        // 1. ĳ������ ���� localScale.x ���� �̿��� �ٶ󺸴� ������ ���մϴ�. (������: 1, ����: -1)
        float facingDirection = -Mathf.Sign(controller.transform.localScale.x);

        // 2. ������ ������ ���ο� ���� �������� ����մϴ�.
        // AttackOffset.x�� facingDirection�� �����༭ ĳ���Ͱ� ������ ���� x �������� ������ �ǵ��� �մϴ�.
        Vector2 offset = new Vector2(MeleeTestWeaponData.AttackOffset.x * facingDirection, MeleeTestWeaponData.AttackOffset.y);

        // 3. ���� ���� �߽� ��ġ�� ����մϴ�.
        Vector2 attackCenter = currentPosition + offset;

        Gizmos.DrawWireCube(attackCenter, MeleeTestWeaponData.BoxSize);
    }
}