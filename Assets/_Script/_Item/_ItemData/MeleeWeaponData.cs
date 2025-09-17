using System.Numerics;

public class MeleeWeaponData : WeaponData
{
    public MeleeWeaponData(string weaponName, string weaponType, int weaponID, float damage, float attackOffset, Vector3 boxSize) : base (weaponName, weaponType, weaponID,damage)
    {
        AttackOffset = attackOffset;
        BoxSize = boxSize;
    }

    public float AttackOffset { get; private set; }
    public Vector3 BoxSize { get; private set; }

}