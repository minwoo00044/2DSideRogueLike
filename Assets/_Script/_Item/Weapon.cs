using UnityEngine;

public abstract class Weapon : Item<WeaponData>, IWeapon
{
    public void Attack(PlayerController owner)
    {
        _Attack(owner);
    }

    protected abstract void _Attack(PlayerController owner);

    public override void Equip(PlayerController owner)
    {
        base.Equip(owner);
        owner.OnAttack += Attack;
        owner.weapon = this;
        Debug.Log("equip");
    }

    public override void Unequip(PlayerController owner)
    {
        if (owner != null)
        {
            owner.OnAttack -= Attack;
        }
        owner = null;

    }

}
