using UnityEngine;

public abstract class Weapon : Item<WeaponData>, IWeapon
{
    public override void Equip(WeaponData data)
    {
    }
    public void Attack(PlayerController owner)
    {
        _Attack(owner);
    }
    protected abstract void _Attack(PlayerController owner);
}
