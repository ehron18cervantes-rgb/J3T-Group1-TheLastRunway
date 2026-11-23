using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 1;

    private int currentHealth;
    private bool isInvincible = false;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(int amount)
    {
        if (isDead || isInvincible) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ActivateShield(float duration)
    {
        StartCoroutine(ShieldRoutine(duration));
    }

    private IEnumerator ShieldRoutine(float duration)
    {
        isInvincible = true;
        Debug.Log("Shield Active!");

        yield return new WaitForSecondsRealtime(duration);

        isInvincible = false;
        Debug.Log("Shield Expired!");
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player Die() called!");

        if (DefeatManager.Instance != null)
        {
            DefeatManager.Instance.ShowDefeat();
        }
        else
        {
            Debug.LogError("ERROR: DefeatManager.Instance is NULL!");
        }

        // 1. Hide sprite visual
        SpriteRenderer sr = (SpriteRenderer)GetComponent(typeof(SpriteRenderer));
        if (sr != null) sr.enabled = false;

        // 2. Disable physics collider
        Collider2D col = (Collider2D)GetComponent(typeof(Collider2D));
        if (col != null) col.enabled = false;

        // 3. Freeze movement / physics simulation
        Rigidbody2D rb = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));
        if (rb != null) rb.simulated = false;
    }
}