using UnityEngine;

public class ZombieSpitfire : MonoBehaviour
{
    public float speed = 7f;
    public float lifetime = 3f;
    public int damage = 10;

    private Vector2 moveDirection;

    void Start()
    {
        // Destroy after a few seconds to prevent lag
        Destroy(gameObject, lifetime);

        // Find the player when the spitfire spawns
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // If player is to the right, fly right and flip the graphic
            if (player.transform.position.x > transform.position.x)
            {
                moveDirection = Vector2.right;
                transform.localScale = new Vector3(-1, 1, 1);
            }
            // If player is to the left, fly left
            else
            {
                moveDirection = Vector2.left;
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            moveDirection = Vector2.left; // Fallback
        }
    }

    void Update()
    {
        // Move in the chosen direction automatically
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            PlayerHealth playerHealth = hitInfo.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}