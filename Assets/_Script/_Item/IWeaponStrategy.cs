using UnityEngine;

// 무기의 행동을 정의하는 인터페이스
public interface IWeaponStrategy
{
    void Attack(PlayerController owner, ItemData data);
    void DrawGizmos(Transform ownerTransform, ItemData data);
}