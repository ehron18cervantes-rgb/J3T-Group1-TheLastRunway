using UnityEngine;

public class CarKeyPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemName = "CarKey"; 
    public string uniqueID = "Lvl3_YellowCarKey"; // Change this for different keys

    private void Start()
    {
        // Prevents the key from respawning if already in inventory
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
                // Adds "CarKey" to your 3-slot inventory
                InventorySystem.Instance.AddItem(itemName, uniqueID);
                Debug.Log("Picked up: " + itemName);
                Destroy(gameObject);
            }
        }
    }
}