using UnityEngine;
public enum ItemType
{
    Weapons,
    Armor,
}
public class Item
{
    public ItemData Data { get; private set; }
    public PlayerController Owner { get; private set; }
    public Item(PlayerController owner)
    {
        Owner = owner;
    }
    public virtual void Equip(ItemData data)
    {
        if (data == null) return;
        Data = data;
    }
    public virtual void Unequip()
    {
    }   
}
