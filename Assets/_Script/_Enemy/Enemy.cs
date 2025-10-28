using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    public float tempHealth = 100f;
    private float tempCrrentHealth;
    private void Start()
    {
        tempCrrentHealth = tempHealth;
    }
    public void TakeDamage(float damage)
    {
        tempCrrentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage, current health: {tempCrrentHealth}");
        if (tempCrrentHealth <= 0)
        {
            Die();
        }
    }
    //testcode
    private void Die()
    {
        Debug.Log("Enemy died.");
        // Add death logic here (e.g., play animation, drop loot, etc.)
        Destroy(gameObject);
    }
}