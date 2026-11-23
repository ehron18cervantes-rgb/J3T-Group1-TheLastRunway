using UnityEngine;

public class ReturnSpawner : MonoBehaviour
{
    void Start()
    {
        if (!string.IsNullOrEmpty(SceneTransition.lastExitUsed))
        {
            GameObject exitPoint = GameObject.Find(SceneTransition.lastExitUsed);

            if (exitPoint != null)
            {
                Vector3 pos = exitPoint.transform.position;
                pos.z = transform.position.z; // keep the player's own depth

                transform.position = pos;

                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null) rb.position = pos;
            }

            // Clear it so a normal restart uses the default spawn spot
            SceneTransition.lastExitUsed = "";
        }
    }
}