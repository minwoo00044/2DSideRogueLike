using UnityEngine;
public enum ItemType
{
    Weapons,
    Armor,
}
public abstract class Item<T> where T : ItemData
{
    protected T Data { get; private set; }
    public PlayerController Owner { get; private set; }
    public virtual void Equip(T data)
    {
        if (data == null) return;
        Data = data;
    }

    public abstract void Unequip();
    public void Init(PlayerController owner)
    {
        if (owner == null) return;
        Owner = owner;
    }
}
