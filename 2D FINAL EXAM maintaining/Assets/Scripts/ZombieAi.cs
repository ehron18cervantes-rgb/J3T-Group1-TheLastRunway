using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    [Header("Detection & Movement")]
    public Transform player;
    public float detectionRange = 8f;
    public float moveSpeed = 2f;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float waypointThreshold = 0.2f;
    private int currentPointIndex = 0;

    [Header("Combat")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;
    public float shootingRange = 5f;
    private float nextFireTime;

    [Header("Sprite")]
    public bool spriteDefaultFacesRight = true; // Set false if your sprite faces LEFT by default

    private Rigidbody2D rb;
    private bool isTouchingPlayer = false;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null || isTouchingPlayer) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            MoveToward(player.position);

            if (distanceToPlayer <= shootingRange && Time.time >= nextFireTime)
                Shoot();
        }
        else if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPointIndex];
        MoveToward(target.position);

        if (Vector2.Distance(transform.position, target.position) < waypointThreshold)
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
    }

    void MoveToward(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;

        // Determine if we should be facing right
        bool shouldFaceRight = direction.x > 0f;

        // Only flip if direction actually changed
        if (shouldFaceRight != facingRight)
            Flip(shouldFaceRight);

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    void Flip(bool faceRight)
    {
        facingRight = faceRight;

        // Flip based on whether the sprite's DEFAULT direction is right or left
        float sign = (faceRight == spriteDefaultFacesRight) ? 1f : -1f;
        transform.localScale = new Vector3(
            sign * Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            nextFireTime = Time.time + fireRate;
        }
    }

private void OnCollisionEnter2D(Collision2D col)
{
    if (col.gameObject.CompareTag("Player"))
    {
        isTouchingPlayer = true;
        PlayerHealth ph = col.gameObject.GetComponent<PlayerHealth>();
        if (ph != null)
            ph.TakeDamage(1);
    }
}

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player")) isTouchingPlayer = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}