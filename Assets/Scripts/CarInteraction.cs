using UnityEngine;
using TMPro;

public class CarInteraction : MonoBehaviour
{
    [Header("UI Prompts")]
    public GameObject interactUI;   
    public GameObject messagePopup; 
    public TextMeshProUGUI textElement; 
    
    [Header("Messages")]
    public string lockedMessage = "The car is locked. Use the Car Key!";
    public string unlockedMessage = "The car is now open!";

    private bool isPlayerNearby;
    private bool isUnlocked = false;

    void Start()
    {
        interactUI.SetActive(false); 
        if(messagePopup != null) messagePopup.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (isUnlocked)
            {
                Debug.Log("Entering car... Loading Minigame!");
                // Use the exact name of your minigame scene here
                UnityEngine.SceneManagement.SceneManager.LoadScene("MiniGame"); 
            }
            else
            {
                ShowPopup(lockedMessage);
            }
        }
    }

    // Called by InventorySystem when CarKey is used
    public void UnlockCar()
    {
        if (isPlayerNearby)
        {
            isUnlocked = true;
            ShowPopup(unlockedMessage);
            Debug.Log("Car unlocked!");
        }
    }

    private void ShowPopup(string msg)
    {
        if (textElement != null) textElement.text = msg;
        StopAllCoroutines();
        StartCoroutine(PopupRoutine());
    }

    private System.Collections.IEnumerator PopupRoutine()
    {
        messagePopup.SetActive(true);
        yield return new WaitForSeconds(2f);
        messagePopup.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            interactUI.SetActive(false);
        }
    }
}