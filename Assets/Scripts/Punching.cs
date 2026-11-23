using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        // This gets the Animator component from your Player object
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Detects Left Mouse Click (0)
        if (Input.GetMouseButtonDown(0))
        {
            PerformPunch();
        }
    }

    void PerformPunch()
    {
        // This triggers the 'Punch' parameter we made in the Animator
        anim.SetTrigger("Punch");
    }
}