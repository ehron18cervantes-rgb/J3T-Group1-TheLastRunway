using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [Header("Zombie Health")]
    [Min(1)] public int maxHealth = 3;

    private int currentHealth;

    void Awake()
    {
        currentHealth = Mathf.Max(1, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth <= 0)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
            Destroy(gameObject);
    }
}
