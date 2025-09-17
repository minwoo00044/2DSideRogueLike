using UnityEngine;

public abstract class Weapon : Item<WeaponData>, IWeapon
{
    private PlayerController controller;
    public void Attack(PlayerController owner)
    {
        _Attack(owner);
    }

    protected abstract void _Attack(PlayerController owner);

    public override void Equip(PlayerController owner)
    {
        if (owner == null) return;
        controller = owner;
        controller.OnAttack += Attack;
    }

    public override void Unequip(PlayerController owner)
    {
        if (owner != null)
        {
            owner.OnAttack -= Attack;
        }
        controller = null;

    }
}
