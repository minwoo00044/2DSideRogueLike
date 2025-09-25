using UnityEngine;
using UnityEngine.InputSystem.XR;

public abstract class Item<T> where T : ItemData
{
    protected T Data { get; private set; }
    protected PlayerController Owner { get; private set; }

    public virtual void Equip(PlayerController owner)
    {
        if (owner == null) return;
        Owner = owner;
    }

    public abstract void Unequip(PlayerController owner);
    public void Init(T _data)
    {
        this.Data = _data;
    }
}
