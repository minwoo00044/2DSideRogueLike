using UnityEngine;

public abstract class Item<T> : MonoBehaviour where T : ItemData
{
    protected T Data { get; private set; }

    public abstract void Equip(PlayerController owner);
    public abstract void Unequip(PlayerController owner);
    public void Init(T _data)
    {
        this.Data = _data;
    }
}
