using UnityEngine;

public struct EnemyData
{
    public string EnemyName;
    public float MoveSpeed;
    public float AtkRange;
    public float AtkSpeed;
    public float MaxHp;
    public float AtkDamage;

    public EnemyData(string enemyName, float moveSpeed, float atkRange, float atkSpeed, float maxHp, float atkDamage)
    {
        EnemyName = enemyName;
        MoveSpeed = moveSpeed;
        AtkRange = atkRange;
        AtkSpeed = atkSpeed;
        MaxHp = maxHp;
        AtkDamage = atkDamage;
    }
}
