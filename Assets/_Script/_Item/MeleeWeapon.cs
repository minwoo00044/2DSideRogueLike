using UnityEngine;
using UnityEngine.InputSystem.XR;

public class MeleeWeapon : Weapon
{
    public MeleeWeaponData MeleeData => Data as MeleeWeaponData;
    protected override void _Attack(PlayerController owner)
    {
        Vector2 currentPosition = owner.transform.position;

        float facingDirection = -Mathf.Sign(owner.transform.localScale.x);
        Vector2 offset = new Vector2(MeleeData.AttackOffset.x * facingDirection, MeleeData.AttackOffset.y);

        Vector2 attackCenter = currentPosition + offset;
        Physics2D.OverlapBox(attackCenter, MeleeData.BoxSize, 0, LayerMask.NameToLayer("Enemy"));
        Debug.Log("ATK");
    }


}