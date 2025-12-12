using UnityEngine;

public class Weapon : Item
{
    public Weapon(PlayerController owner) : base(owner)
    {
    }

    public void Attack(PlayerController owner)
    {
        if(Data == null) return;
        _Attack(owner);
    }
    protected virtual void _Attack(PlayerController owner)
    {

    }
}
