using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting")]
    public float fireRate = 0.2f;
    public float bulletSpeed = 18f;
    public float bulletLifetime = 4f;

    [Header("Gun Muzzle Position (local space)")]
    [Tooltip("Character 1 gun barrel position.")]
    public Vector2 character1Muzzle = new Vector2(0.31f, 0.03f);

    [Tooltip("Character 2 hand/barrel position.")]
    public Vector2 character2Muzzle = new Vector2(0.30f, 0.06f);

    [Tooltip("Character 3 gun barrel position.")]
    public Vector2 character3Muzzle = new Vector2(0.32f, 0.09f);

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private GameObject bulletPrefab;
    private float nextFireTime;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Load the project's player bullet without requiring Inspector changes.
        bulletPrefab = Resources.Load<GameObject>("PlayerBullet");

        if (bulletPrefab == null)
            Debug.LogError("[PlayerShooting] PlayerBullet prefab could not be loaded from Resources.");
    }

    void Update()
    {
        // Keep the existing control: left mouse / default Fire1.
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (anim != null)
            anim.SetTrigger("Shoot");

        if (bulletPrefab == null)
            return;

        bool facingLeft = spriteRenderer != null && spriteRenderer.flipX;
        Vector2 direction = facingLeft ? Vector2.left : Vector2.right;

        // Use a local-space muzzle point so it follows the player's perspective scaling.
        // This keeps the bullet at the gun/hand instead of spawning near the player's feet.
        Vector2 muzzle = GetMuzzleForSelectedCharacter();
        float localX = facingLeft ? -Mathf.Abs(muzzle.x) : Mathf.Abs(muzzle.x);
        Vector3 spawnPosition = transform.TransformPoint(new Vector3(localX, muzzle.y, 0f));

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        if (playerBullet == null)
            playerBullet = bullet.AddComponent<PlayerBullet>();

        playerBullet.Initialize(direction, bulletSpeed, bulletLifetime, gameObject);
    }

    private Vector2 GetMuzzleForSelectedCharacter()
    {
        switch (CharacterSelection.GetCharacter())
        {
            case 1:
                return character2Muzzle;
            case 2:
                return character3Muzzle;
            default:
                return character1Muzzle;
        }
    }
}