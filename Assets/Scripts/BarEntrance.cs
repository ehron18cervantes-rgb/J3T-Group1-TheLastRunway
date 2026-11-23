using UnityEngine;

using UnityEngine.SceneManagement;



public class SceneTransition : MonoBehaviour

{

    public string sceneToLoad;      

    public GameObject interactUI;  

    public string CinemaEntrance; // In Scene 3 Cinema, type "Cinema" here in the Inspector

    private bool isPlayerNearby;



    // This variable "survives" the scene change in the computer's memory

    public static string lastExitUsed;



    void Start()

    {

        interactUI.SetActive(false);

    }



    void Update()

    {

        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))

        {

            // Save the exit name before the scene changes

            lastExitUsed = CinemaEntrance;

            SceneManager.LoadScene(sceneToLoad);

        }

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