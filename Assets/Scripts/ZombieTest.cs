using UnityEngine;

public class ZombieTest : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        // This links the script to the Animator component you added earlier
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // When you hold down the Right Arrow key, the boolean turns true and he walks
        if (Input.GetKey(KeyCode.RightArrow))
        {
            anim.SetBool("isWalking", true);

            // This line actually physically moves him forward across the screen
            transform.Translate(Vector2.right * 2f * Time.deltaTime);
        }
        // When you let go of the key, the boolean turns false and he returns to Idle
        else
        {
            anim.SetBool("isWalking", false);
        }
    }
}