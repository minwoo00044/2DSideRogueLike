public abstract class WeaponData : ItemData
{
    // [중요] 리플렉션용 빈 생성자 추가
    public WeaponData() { }

    public WeaponData(string itemName, ItemType itemType, int itemID, float damage) : base(itemName, itemType, itemID)
    {
        Damage = damage;
    }

    // [중요] 필드로 변경
    public float Damage;
}