// 근거리 공격 전략 (기존 MeleeWeapon 로직 이동)
using UnityEngine;

public class MeleeStrategy : IWeaponStrategy
{
    public void Attack(PlayerController owner, ItemData data)
    {
        // 데이터 캐스팅 (안전하게)
        var meleeData = data as MeleeWeaponData;
        if (meleeData == null) return;

        Vector2 currentPosition = owner.transform.position;
        float facingDirection = -Mathf.Sign(owner.transform.localScale.x);
        Vector2 offset = new Vector2(meleeData.AttackOffset.x * facingDirection, meleeData.AttackOffset.y);
        Vector2 attackCenter = currentPosition + offset;

        var target = Physics2D.OverlapBox(attackCenter, meleeData.BoxSize, 0, LayerMask.GetMask("Enemy"));

        if (target != null)
            target.GetComponent<IDamagable>()?.TakeDamage(meleeData.Damage);

        Debug.Log($"Melee Attack: {data.ItemName}");
    }

    public void DrawGizmos(Transform ownerTransform, ItemData data)
    {
        var meleeData = data as MeleeWeaponData;
        if (meleeData == null) return;

        Gizmos.color = Color.red;
        float facingDirection = -Mathf.Sign(ownerTransform.localScale.x);
        Vector2 offset = new Vector2(meleeData.AttackOffset.x * facingDirection, meleeData.AttackOffset.y);
        Gizmos.DrawWireCube((Vector2)ownerTransform.position + offset, meleeData.BoxSize);
    }
}
