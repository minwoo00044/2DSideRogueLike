public abstract class ItemData
{
    public ItemData(string weaponName, string weaponType, int weaponID)
    {
        WeaponName = weaponName;
        WeaponType = weaponType;
        WeaponID = weaponID;
    }
    public string WeaponName { get; private set; }
    public string WeaponType { get; private set; }
    public int WeaponID { get; private set; }
}