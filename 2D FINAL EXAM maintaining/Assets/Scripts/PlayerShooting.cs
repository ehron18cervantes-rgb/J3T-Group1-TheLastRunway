using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Check if you are HOLDING the mouse button (Input.GetButton)
        // This is better than GetButtonDown for automatic/sustained firing
        if (Input.GetButton("Fire1"))
        {
            Shoot();
        }
        else
        {
            // 2. Tell the animator to stop the shooting loop when you let go
            anim.SetBool("Shoot", false);
        }
    }

    void Shoot()
    {
        // Instead of a Trigger, we use a Bool so it stays in the "Shooting" state
        anim.SetBool("Shoot", true);
        
        // Muzzle and bullet instantiation logic has been removed as requested.
    }
}