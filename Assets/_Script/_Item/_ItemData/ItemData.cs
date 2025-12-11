public abstract class ItemData
{
    // [중요] 리플렉션용 빈 생성자 추가
    public ItemData() { }

    public ItemData(string itemName, ItemType itemType, int weaponID)
    {
        ItemName = itemName;
        ItemType = itemType; // enum을 string으로 변환
        ItemID = weaponID;
    }

    // [중요] GetField가 찾을 수 있도록 '필드'로 변경
    public string ItemName;
    public ItemType ItemType;
    public int ItemID;
}