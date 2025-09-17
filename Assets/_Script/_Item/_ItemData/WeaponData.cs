public abstract class WeaponData : ItemData
{
    public WeaponData(string itemName, string itemType, int itemID, float damage) : base(itemName, itemType, itemID)
    {
        Damage = damage;
    }
    public float Damage { get; private set; }
}