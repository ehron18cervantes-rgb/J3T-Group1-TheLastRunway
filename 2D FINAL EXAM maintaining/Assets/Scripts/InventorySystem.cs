using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    // Added for Scene 3 Cinema Spawn Logic
    public static string lastExitName; 

    [Header("Setup")]
    public SpriteRenderer[] slotRenderers;
    public Sprite keySprite, shieldSprite, potionSprite, carKeySprite; 
    
    [Header("Settings")]
    public Vector3 offsetFromCamera = new Vector3(-5f, -3f, 10f);

    private string[] itemsInSlots = new string[3];
    private List<string> pickedUpItemIDs = new List<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Camera.main != null)
        {
            transform.position = Camera.main.transform.position + offsetFromCamera;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseItem(2);
    }

    public void AddItem(string itemName, string id)
    {
        pickedUpItemIDs.Add(id);

        for (int i = 0; i < slotRenderers.Length; i++)
        {
            if (slotRenderers[i].sprite == null)
            {
                itemsInSlots[i] = itemName;
                
                if (itemName == "Key") slotRenderers[i].sprite = keySprite;
                else if (itemName == "Shield") slotRenderers[i].sprite = shieldSprite;
                else if (itemName == "Potion") slotRenderers[i].sprite = potionSprite;
                else if (itemName == "CarKey") slotRenderers[i].sprite = carKeySprite; 
                
                return;
            }
        }
    }

    public bool HasItem(string itemName)
    {
        for (int i = 0; i < itemsInSlots.Length; i++)
        {
            if (itemsInSlots[i] == itemName) return true;
        }
        return false;
    }

    public bool IsItemAlreadyPickedUp(string id)
    {
        return pickedUpItemIDs.Contains(id);
    }

    void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= itemsInSlots.Length || itemsInSlots[slotIndex] == null) return;

        string usedItem = itemsInSlots[slotIndex];
        Debug.Log("Using item: " + usedItem);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        bool itemConsumed = false;

        if (usedItem == "Key")
        {
            lvl1exit exitDoor = Object.FindFirstObjectByType<lvl1exit>();
            if (exitDoor != null)
            {
                exitDoor.UnlockDoor();
                itemConsumed = true; 
                Debug.Log("Key used on door.");
            }
        }
        else if (usedItem == "CarKey")
        {
            CarInteraction car = Object.FindFirstObjectByType<CarInteraction>();
            if (car != null)
            {
                car.UnlockCar();
                itemConsumed = true;
                Debug.Log("Car Key used.");
            }
        }
        else if (usedItem == "Shield")
        {
            if (player != null)
            {
                PlayerHealth health = player.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.ActivateShield(2f); // 2 second invincibility
                    itemConsumed = true;
                }
            }
        }
        else if (usedItem == "Potion") 
        {
            if (player != null)
            {
                PlayerController controller = player.GetComponent<PlayerController>();
                if (controller != null)
                {
                    controller.ApplySpeedBoost(2f, 3f); 
                    itemConsumed = true;
                }
            }
        }

        // Only clear slot and visual sprite if the item was successfully consumed
        if (itemConsumed)
        {
            slotRenderers[slotIndex].sprite = null;
            itemsInSlots[slotIndex] = null;
        }
    }
}