using UnityEngine;

public class MovableFloor : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // Get input from WASD or Arrow Keys
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Calculate the movement vector
        Vector3 movement = new Vector3(moveX, moveY, 0f) * speed * Time.deltaTime;

        // Apply the movement to the object's position
        transform.position += movement;
    }
}