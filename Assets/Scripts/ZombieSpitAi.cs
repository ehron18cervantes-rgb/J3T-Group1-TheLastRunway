using System.Collections;
using UnityEngine;

public class ZombieSpitAI : MonoBehaviour
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
    [Tooltip("Cooldown (seconds) between the end of one spit and the start of the next charge.")]
    public float fireRate = 1.5f;
    public float shootingRange = 5f;
    [Tooltip("Extra range allowed once charging has begun, so the zombie doesn't cancel the instant the player steps back a hair. 1.25 = 25% extra.")]
    public float chargeCancelRangeMultiplier = 1.25f;
    private float nextFireTime;

    [Header("Charge-Up Timing")]
    [Tooltip("Length of the Charge-Up clip in seconds (clip length / animator speed).")]
    public float chargeDuration = 0.8f;
    [Tooltip("ON: bullet is spawned by an Animation Event calling AnimEvent_Fire(). OFF: bullet is spawned by a timer.")]
    public bool useAnimationEvent = true;
    [Tooltip("Only used when useAnimationEvent is OFF. Seconds into the charge when the bullet is released.")]
    public float timerReleaseTime = 0.7f;

    [Header("Sprite")]
    public bool spriteDefaultFacesRight = true;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isTouchingPlayer = false;
    private bool facingRight = true;

    // Charge state
    private bool isCharging = false;
    private bool fireEventReceived = false;
    private Coroutine chargeRoutine;

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
        // While charging, the coroutine owns the zombie. Stand still.
        if (isCharging) return;

        if (player == null || isTouchingPlayer)
        {
            SetWalking(false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > shootingRange)
            {
                // Chase
                MoveToward(player.position);
                SetWalking(true);
            }
            else
            {
                // In range: stop, face the player, start charging when off cooldown
                SetWalking(false);
                FacePlayer();

                if (Time.time >= nextFireTime)
                    StartCharge();
            }
        }
        else if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Patrol();
            SetWalking(true);
        }
        else
        {
            SetWalking(false);
        }
    }

    // ---------------------------------------------------------------
    // Charge / Spit
    // ---------------------------------------------------------------

    void StartCharge()
    {
        if (isCharging) return;
        chargeRoutine = StartCoroutine(ChargeAndSpit());
    }

    IEnumerator ChargeAndSpit()
    {
        isCharging = true;
        fireEventReceived = false;

        SetWalking(false);
        if (anim != null) anim.SetBool("Spit", true);

        float t = 0f;
        bool fired = false;

        while (t < chargeDuration)
        {
            // Interrupt if the player escaped or we're now in melee contact
            if (!CanKeepCharging())
            {
                CancelCharge();
                yield break;
            }

            FacePlayer();

            if (!fired)
            {
                bool shouldRelease = useAnimationEvent ? fireEventReceived : t >= timerReleaseTime;
                if (shouldRelease)
                {
                    Shoot();
                    fired = true;
                }
            }

            t += Time.deltaTime;
            yield return null;
        }

        // Safety net: if the animation event never fired (missing/misplaced), still spit.
        if (!fired && CanKeepCharging())
            Shoot();

        EndCharge();
    }

    bool CanKeepCharging()
    {
        if (player == null || isTouchingPlayer) return false;
        float dist = Vector2.Distance(transform.position, player.position);
        return dist <= shootingRange * chargeCancelRangeMultiplier;
    }

    // Player escaped or touched us before release: no bullet, no full cooldown.
    void CancelCharge()
    {
        isCharging = false;
        chargeRoutine = null;
        if (anim != null) anim.SetBool("Spit", false);
        nextFireTime = Time.time + 0.25f; // tiny delay so it doesn't re-trigger on the same frame
    }

    // Charge finished normally: apply the full cooldown.
    void EndCharge()
    {
        isCharging = false;
        chargeRoutine = null;
        if (anim != null) anim.SetBool("Spit", false);
        nextFireTime = Time.time + fireRate;
    }

    // Called by an Animation Event on the Charge-Up clip (near the last frame).
    public void AnimEvent_Fire()
    {
        // Ignore stray events (e.g. if the clip is previewed or blended while not charging)
        if (isCharging) fireEventReceived = true;
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning($"{name}: Bullet Prefab or Fire Point is not assigned, so nothing was spat.", this);
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        ZombieSpit spit = bullet.GetComponent<ZombieSpit>();
        if (spit != null)
            spit.SetDirection(facingRight ? Vector2.right : Vector2.left);
    }

    // ---------------------------------------------------------------
    // Movement helpers
    // ---------------------------------------------------------------

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

    void FacePlayer()
    {
        if (player == null) return;
        bool playerIsRight = player.position.x > transform.position.x;
        if (playerIsRight != facingRight)
            Flip(playerIsRight);
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

    void SetWalking(bool value)
    {
        if (anim != null) anim.SetBool("isWalking", value);
    }

    // ---------------------------------------------------------------
    // Collision
    // ---------------------------------------------------------------

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
            SetWalking(false);

            // Melee contact interrupts any charge in progress
            if (isCharging)
            {
                if (chargeRoutine != null) StopCoroutine(chargeRoutine);
                CancelCharge();
            }

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

    void OnDisable()
    {
        // Reset state if the zombie is disabled/pooled mid-charge
        isCharging = false;
        chargeRoutine = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, shootingRange * chargeCancelRangeMultiplier);

        // Shows exactly where the spit will spawn
        if (firePoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(firePoint.position, 0.12f);
        }
    }
}