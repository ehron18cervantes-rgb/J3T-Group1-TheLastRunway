using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Tooltip("Must be Key, Shield, or Potion")]
    public string itemName;

    [Tooltip("Give every single item in the game a unique ID (e.g., StreetKey1)")]
    public string uniqueID;

    void Start()
    {
        // Check if this specific item was already picked up in a previous visit to this scene
        if (InventorySystem.Instance != null && InventorySystem.Instance.IsItemAlreadyPickedUp(uniqueID))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (InventorySystem.Instance != null)
            {
                InventorySystem.Instance.AddItem(itemName, uniqueID);
                Destroy(gameObject);
                Debug.Log(itemName + " added to persistent inventory.");
            }
            else
            {
                Debug.LogError("InventorySystem not found!");
            }
        }
    }
}