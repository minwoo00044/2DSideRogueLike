public class MeleeWeapon : Weapon
{
    private MeleeWeaponData MeleeData => Data as MeleeWeaponData;
    protected override void _Attack(PlayerController owner)
    {
    }
}