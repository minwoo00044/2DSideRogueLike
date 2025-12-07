public abstract class ItemData
{
    // [중요] 리플렉션용 빈 생성자 추가
    public ItemData() { }

    public ItemData(string itemName, string itemType, int weaponID)
    {
        ItemName = itemName;
        ItemType = itemType;
        ItemID = weaponID;
    }

    // [중요] GetField가 찾을 수 있도록 '필드'로 변경
    public string ItemName;
    public string ItemType;
    public int ItemID;
}