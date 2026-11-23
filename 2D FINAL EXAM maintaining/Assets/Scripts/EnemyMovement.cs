using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    public Transform[] patrolPoints;
    public float speed = 2f;
    private int currentPointIndex = 0;

    [Header("Detection")]
    public float detectionRange = 8f;
    public float shootingRange = 5f;
    private Transform player;

    [Header("Combat")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;
    private float nextFireTime;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update() {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= shootingRange) {
            AttackPlayer();
        } else if (distanceToPlayer <= detectionRange) {
            FollowPlayer();
        } else {
            Patrol();
        }
    }

    void Patrol() {
        Transform targetPoint = patrolPoints[currentPointIndex];
        MoveTowards(targetPoint.position);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f) {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    void FollowPlayer() {
        MoveTowards(player.position);
    }

    void AttackPlayer() {
        // Stay still while shooting or move slowly
        if (Time.time >= nextFireTime) {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            nextFireTime = Time.time + fireRate;
        }
    }

    void MoveTowards(Vector2 target) {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        
        // Flip sprite to face movement direction
        Vector3 localScale = transform.localScale;
        if (target.x > transform.position.x) localScale.x = Mathf.Abs(localScale.x);
        else localScale.x = -Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }
}