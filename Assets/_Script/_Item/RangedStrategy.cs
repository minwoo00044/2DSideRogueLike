using UnityEngine;

public class RangedStrategy : IWeaponStrategy
{
    public void Attack(PlayerController owner, ItemData data)
    {
        // 원거리 데이터(RangedWeaponData)가 있다고 가정
        // var rangedData = data as RangedWeaponData; 

        Debug.Log("화살(투사체)을 발사합니다!");
        // 여기에 Instantiate 로직이나 ObjectPooling 로직 작성
    }

    public void DrawGizmos(Transform ownerTransform, ItemData data)
    {
        // 원거리는 사거리 표시 등을 할 수 있음
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ownerTransform.position, 5.0f);
    }
}