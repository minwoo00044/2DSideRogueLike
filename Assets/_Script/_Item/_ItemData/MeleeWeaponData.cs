using UnityEngine;

public class MeleeWeaponData : WeaponData
{
    public MeleeWeaponData(string weaponName, string weaponType, int weaponID, float damage, Vector2 attackOffset, Vector2 boxSize) : base (weaponName, weaponType, weaponID,damage)
    {
        AttackOffset = attackOffset;
        BoxSize = boxSize;
    }

    public Vector2 AttackOffset { get; private set; }
    public Vector2 BoxSize { get; private set; }

}