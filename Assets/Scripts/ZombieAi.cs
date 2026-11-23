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
    public bool spriteDefaultFacesRight = true;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isTouchingPlayer = false;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (GetComponent<ZombieHealth>() == null)
            gameObject.AddComponent<ZombieHealth>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null || isTouchingPlayer)
        {
            if (anim != null) anim.SetBool("isWalking", false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // If we are farther than shooting range, walk toward player.
            // Once within shooting range, STOP completely and hold position to spit!
            if (distanceToPlayer > shootingRange)
            {
                MoveToward(player.position);
                if (anim != null) anim.SetBool("isWalking", true);
            }
            else
            {
                // Stop walking and hold ground to face/shoot the player
                if (anim != null) anim.SetBool("isWalking", false);

                // Ensure it faces the player even while standing still
                if (player.position.x > transform.position.x != facingRight)
                {
                    Flip(player.position.x > transform.position.x);
                }
            }

            if (distanceToPlayer <= shootingRange && Time.time >= nextFireTime)
            {
                Shoot();
            }
        }
        else if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Patrol();
            if (anim != null) anim.SetBool("isWalking", true);
        }
        else
        {
            if (anim != null) anim.SetBool("isWalking", false);
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
        bool shouldFaceRight = direction.x > 0f;

        if (shouldFaceRight != facingRight)
            Flip(shouldFaceRight);

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    void Flip(bool faceRight)
    {
        facingRight = faceRight;
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
            if (anim != null) anim.SetBool("isWalking", false);

            PlayerHealth ph = col.gameObject.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(1);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
            isTouchingPlayer = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}