using UnityEngine;
using UnityEngine.InputSystem.XR;

public class MeleeWeapon : Weapon,IGizmoDrawable
{
    public MeleeWeapon(PlayerController owner) : base(owner)
    {
    }

    public MeleeWeaponData MeleeData => Data as MeleeWeaponData;
    protected override void _Attack(PlayerController owner)
    {
        Vector2 currentPosition = owner.transform.position;

        float facingDirection = -Mathf.Sign(owner.transform.localScale.x);
        Vector2 offset = new Vector2(MeleeData.AttackOffset.x * facingDirection, MeleeData.AttackOffset.y);

        Vector2 attackCenter = currentPosition + offset;
        var target = Physics2D.OverlapBox(attackCenter, MeleeData.BoxSize, 0, LayerMask.GetMask("Enemy"));
        if(target != null)
            target.GetComponent<IDamagable>().TakeDamage(MeleeData.Damage);
        else
             Debug.Log("No enemy hit");
        Debug.Log(Data.ItemName);
    }
    public void DrawGizmos(Transform ownerTransform)
    {
        if (Owner == null) return;
        Gizmos.color = Color.red;

        Vector2 currentPosition = ownerTransform.position;

        // 1. 캐릭터의 현재 localScale.x 값을 이용해 바라보는 방향을 구합니다. (오른쪽: 1, 왼쪽: -1)
        float facingDirection = -Mathf.Sign(ownerTransform.localScale.x);

        // 2. 방향을 적용한 새로운 공격 오프셋을 계산합니다.
        // AttackOffset.x에 facingDirection을 곱해줘서 캐릭터가 왼쪽을 보면 x 오프셋이 음수가 되도록 합니다.
        Vector2 offset = new Vector2(MeleeData.AttackOffset.x * facingDirection, MeleeData.AttackOffset.y);

        // 3. 최종 공격 중심 위치를 계산합니다.
        Vector2 attackCenter = currentPosition + offset;

        Gizmos.DrawWireCube(attackCenter, MeleeData.BoxSize);
    }
    public override void Equip(ItemData data)
    {
        base.Equip(data);
        if (data == null) return;
        if (Owner == null) return;
        Owner.AddBuff(MeleeData.Damage, 0);
    }
    public override void Unequip()
    {
        throw new System.NotImplementedException();
    }
}