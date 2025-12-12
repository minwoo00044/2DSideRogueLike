using UnityEngine;

public class MeleeWeaponData : WeaponData
{
    // 이제 부모 생성자가 존재하므로 에러가 나지 않음
    public MeleeWeaponData() { }

    public MeleeWeaponData(string itemName, ItemType itemType, int weaponID, float damage, Vector2 attackOffset, Vector2 boxSize) 
        :  base(itemName, itemType, weaponID, damage)
    {
        AttackOffset = attackOffset;
        BoxSize = boxSize;
    }

    // [중요] 필드로 변경
    public Vector2 AttackOffset;
    public Vector2 BoxSize;
}