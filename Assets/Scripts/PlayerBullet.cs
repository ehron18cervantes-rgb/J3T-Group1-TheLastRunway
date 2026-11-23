using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("Damage")]
    [Min(1)] public int damage = 1;

    private Rigidbody2D rb;
    private Collider2D bulletCollider;
    private GameObject owner;
    private bool hasHitZombie;

    public void Initialize(Vector2 direction, float speed, float lifetime, GameObject bulletOwner)
    {
        owner = bulletOwner;
        rb = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<Collider2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.linearVelocity = direction.normalized * speed;
        }

        // Prevent the bullet from hitting the character who fired it.
        if (bulletCollider != null && owner != null)
        {
            Collider2D[] ownerColliders = owner.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D ownerCollider in ownerColliders)
            {
                if (ownerCollider != null)
                    Physics2D.IgnoreCollision(bulletCollider, ownerCollider, true);
            }
        }

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamageZombie(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamageZombie(collision.collider);
    }

    private void TryDamageZombie(Collider2D other)
    {
        if (hasHitZombie || other == null)
            return;

        if (owner != null && (other.gameObject == owner || other.transform.IsChildOf(owner.transform)))
            return;

        ZombieHealth zombieHealth = other.GetComponentInParent<ZombieHealth>();

        // Reliable fallback for existing zombie objects that have ZombieAI but
        // were created before ZombieHealth was added to the project.
        if (zombieHealth == null)
        {
            ZombieAI zombieAI = other.GetComponentInParent<ZombieAI>();
            if (zombieAI != null)
            {
                zombieHealth = zombieAI.GetComponent<ZombieHealth>();
                if (zombieHealth == null)
                    zombieHealth = zombieAI.gameObject.AddComponent<ZombieHealth>();
            }
        }

        // Final fallback for older/manual zombie objects that are named Zombie.
        if (zombieHealth == null)
        {
            Transform current = other.transform;
            while (current != null)
            {
                if (current.name.ToLowerInvariant().Contains("zombie"))
                {
                    zombieHealth = current.GetComponent<ZombieHealth>();
                    if (zombieHealth == null)
                        zombieHealth = current.gameObject.AddComponent<ZombieHealth>();
                    break;
                }

                current = current.parent;
            }
        }

        if (zombieHealth == null)
            return;

        hasHitZombie = true;
        zombieHealth.TakeDamage(damage);
        Destroy(gameObject);
    }
}
