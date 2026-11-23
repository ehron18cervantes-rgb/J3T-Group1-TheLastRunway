using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2.5f; 

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    private Vector2 moveInput;
    private bool isDead = false;

    // --- Added for Potion ---
    private float originalSpeed;
    private bool isSpeedBoosted = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // Ensure physics doesn't interfere with top-down movement
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        // --- Store the starting speed ---
        originalSpeed = moveSpeed;
    }

    void Update()
    {
        if (isDead) return;

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        anim.SetBool("isWalking", isMoving);

        if (moveInput.x > 0) sr.flipX = false;
        else if (moveInput.x < 0) sr.flipX = true;

        if (Input.GetKeyDown(KeyCode.K)) 
        {
            anim.SetTrigger("Hurt");
        }

        if (Input.GetKeyDown(KeyCode.L)) 
        {
            isDead = true;
            anim.SetBool("Dead", true);
            rb.linearVelocity = Vector2.zero; 
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        float verticalDamping = 0.5f;
        Vector2 direction = new Vector2(moveInput.x, moveInput.y * verticalDamping);

        if (direction.magnitude > 1)
        {
            direction.Normalize();
        }

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    // --- NEW FUNCTIONS FOR POTION BOOST ---

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        if (!isSpeedBoosted) 
        {
            StartCoroutine(SpeedBoostRoutine(multiplier, duration));
        }
    }

    private System.Collections.IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        isSpeedBoosted = true;
        moveSpeed *= multiplier; // Temporarily increase speed
        Debug.Log("Speed Boost Active!");

        yield return new WaitForSeconds(duration);

        moveSpeed = originalSpeed; // Reset to 2.5f
        isSpeedBoosted = false;
        Debug.Log("Speed Boost Expired.");
    }
}