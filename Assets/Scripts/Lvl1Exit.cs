using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class lvl1exit : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad;
    public bool requiresKey = false;
    public string doorID; // e.g. "CinemaDoor" so the key knows which door to open

    [Header("UI Prompts")]
    public GameObject interactUI;
    public GameObject messagePopup;
    public TextMeshProUGUI textElement;

    [Header("Messages")]
    [TextArea] public string lockedMessage = "The door is locked. Press 1 to use the key from your inventory!";
    [TextArea] public string noKeyMessage = "You need a key to unlock this door.";
    [TextArea] public string unlockedMessage = "Door Unlocked! Press E to enter.";

    private bool isPlayerNearby;
    private bool isUnlocked = false; // true only after the key is actually used

    void Start()
    {
        if (interactUI != null) interactUI.SetActive(false);
        if (messagePopup != null) messagePopup.SetActive(false);

        // If the door never needed a key, it starts unlocked
        if (!requiresKey) isUnlocked = true;
    }

    void Update()
    {
        if (!isPlayerNearby) return;

        // Press 1 near the door to use the key
        if (requiresKey && !isUnlocked && Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (InventorySystem.Instance != null && InventorySystem.Instance.HasItem("Key"))
            {
                UnlockDoor();
            }
            else
            {
                ShowText(noKeyMessage);
            }
        }

        // Press E to enter, but only if the door has been unlocked
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isUnlocked)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                ShowText(lockedMessage);
            }
        }
    }

    // Can also be called manually by an item click if needed
    public void UnlockDoor()
    {
        if (isUnlocked) return;

        isUnlocked = true;
        ShowText(unlockedMessage);
        Debug.Log("Door opened with key!");
    }

    private void ShowText(string message)
    {
        if (textElement != null) textElement.text = message;
        StopAllCoroutines();
        StartCoroutine(ShowMessage());
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