using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;


    [Header("UI Icons")]
    public GameObject keyIcon;
    public GameObject shieldIcon;
    public GameObject potionIcon; // Changed from flashlight


    [Header("UI Highlights")]
    public GameObject keyHighlight;
    public GameObject shieldHighlight;
    public GameObject potionHighlight; // Changed from flashlight


    // Track if player has picked them up
    public bool hasKey, hasShield, hasPotion;


    private void Awake()
    {
        Instance = this;
    }


    private void Update()
    {
        // Highlight logic for pressing 1, 2, or 3
        if (Input.GetKeyDown(KeyCode.Alpha1) && hasKey) TriggerHighlight(keyHighlight);
        if (Input.GetKeyDown(KeyCode.Alpha2) && hasShield) TriggerHighlight(shieldHighlight);
        if (Input.GetKeyDown(KeyCode.Alpha3) && hasPotion) TriggerHighlight(potionHighlight);
    }


    public void AddItem(string itemName)
    {
        string name = itemName.ToLower().Trim();
       
        if (name == "key") { hasKey = true; keyIcon.SetActive(true); }
        if (name == "shield") { hasShield = true; shieldIcon.SetActive(true); }
        if (name == "potion") { hasPotion = true; potionIcon.SetActive(true); }
       
        Debug.Log("Picked up: " + name);
    }


    private void TriggerHighlight(GameObject highlightObj)
    {
        // Turn off all highlights first so only one glows at a time
        keyHighlight.SetActive(false);
        shieldHighlight.SetActive(false);
        potionHighlight.SetActive(false);


        // Turn on the selected one
        highlightObj.SetActive(true);
    }
}
