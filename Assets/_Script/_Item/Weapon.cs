using UnityEngine;

// IGizmoDrawable 인터페이스도 Weapon이 구현해서 Strategy에게 넘겨줍니다.
public class Weapon : Item, IGizmoDrawable
{
    private IWeaponStrategy _currentStrategy;

    public Weapon(PlayerController owner) : base(owner)
    {
        owner.OnAttack += Attack;
    }

    // ★ 가장 중요한 부분: 데이터가 들어올 때 행동(Strategy)을 결정합니다.
    public override void Equip(ItemData data)
    {
        base.Equip(data); // Data = data 설정됨

        if (data is MeleeWeaponData)
        {
            _currentStrategy = new MeleeStrategy();
            // 필요하다면 여기서 버프 적용 (MeleeWeapon에 있던 로직)
            if (Owner != null) Owner.AddBuff((data as MeleeWeaponData).Damage, 0);
        }
        //else if (data is RangedWeaponData) // RangedWeaponData 클래스가 있다고 가정
        //{
        //    _currentStrategy = new RangedStrategy();
        //}
        else
        {
            Debug.LogWarning("알 수 없는 무기 데이터 타입입니다.");
            _currentStrategy = null;
        }

        Debug.Log($"Equip Weapon: {data.ItemName}");
    }

    public void Attack(PlayerController owner)
    {
        if (Data == null || _currentStrategy == null) return;

        // 현재 전략에게 공격 위임
        _currentStrategy.Attack(owner, Data);
    }

    // Gizmo 그리기 위임
    public void DrawGizmos(Transform ownerTransform)
    {
        if (Data == null || _currentStrategy == null) return;

        _currentStrategy.DrawGizmos(ownerTransform, Data);
    }

    public override void Unequip()
    {
        // 버프 해제 등의 로직
        _currentStrategy = null;
        base.Unequip();
    }
}