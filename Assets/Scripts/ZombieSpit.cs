using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ZombieSpit : MonoBehaviour
{
    public float speed = 6f;
    public int damage = 1;
    public float lifetime = 4f;

    [Tooltip("Tick if the spit sprite's art points RIGHT. Untick if it points LEFT.")]
    public bool spriteDefaultFacesRight = false;

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.right;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Start()
    {
        rb.linearVelocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    // Called by the zombie right after spawning
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        // Flip the sprite so it leads in the direction of travel
        bool travellingRight = direction.x > 0f;
        bool needsFlip = travellingRight != spriteDefaultFacesRight;

        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * (needsFlip ? -1f : 1f);
        transform.localScale = s;

        if (rb != null) rb.linearVelocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph != null) ph.TakeDamage(damage);

        Destroy(gameObject);
    }
}