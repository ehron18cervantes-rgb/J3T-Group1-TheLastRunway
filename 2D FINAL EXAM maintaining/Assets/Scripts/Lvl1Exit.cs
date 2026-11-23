using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class lvl1exit : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad;      
    public bool requiresKey = false; 
    public string doorID; // Give this a name like "CinemaDoor" so the key knows which door to open

    [Header("UI Prompts")]
    public GameObject interactUI;   
    public GameObject messagePopup; 
    public TextMeshProUGUI textElement; 
    
    [Header("Messages")]
    [TextArea] public string lockedMessage = "The door is locked. Use the key from your inventory!";
    [TextArea] public string unlockedMessage = "Door Unlocked!";

    private bool isPlayerNearby;
    private bool isUnlocked = false; // Tracks if the key was actually used

    void Start()
    {
        if (interactUI != null) interactUI.SetActive(false); 
        if (messagePopup != null) messagePopup.SetActive(false);
        
        // If it never needed a key, it starts unlocked
        if (!requiresKey) isUnlocked = true;
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // Check if InventorySystem exists and has the "Key" item
            bool hasKeyInInventory = false;

            if (InventorySystem.Instance != null && InventorySystem.Instance.HasItem("Key"))
            {
                hasKeyInInventory = true;
            }

            // If door doesn't need key, or was unlocked, or player carries key, change scene
            if (isUnlocked || hasKeyInInventory)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                // Key is required but player doesn't have it
                if (textElement != null) textElement.text = lockedMessage;
                StopAllCoroutines();
                StartCoroutine(ShowMessage());
            }
        }
    }

    // This function can still be called manually by item click if needed
    public void UnlockDoor()
    {
        isUnlocked = true;
        if (textElement != null) textElement.text = unlockedMessage;
        StopAllCoroutines();
        StartCoroutine(ShowMessage());
        Debug.Log("Door opened with key!");
    }

    private System.Collections.IEnumerator ShowMessage()
    {
        if (messagePopup != null)
        {
            messagePopup.SetActive(true);
            if (interactUI != null) interactUI.SetActive(false); 
            yield return new WaitForSeconds(2f);
            messagePopup.SetActive(false);
            if (isPlayerNearby && interactUI != null) interactUI.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactUI != null) interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactUI != null) interactUI.SetActive(false);
            if (messagePopup != null) messagePopup.SetActive(false);
        }
    }
}