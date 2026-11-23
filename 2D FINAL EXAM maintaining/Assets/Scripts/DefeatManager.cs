using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DefeatManager : MonoBehaviour
{
    public static DefeatManager Instance;

    [Header("UI References")]
    public GameObject defeatPanel;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI bestTimeText;

    [Header("Scene Names")]
    public string menuSceneName = "MainMenu";

    private float surviveTime;
    private bool gameOver = false;

    void Awake()
    {
        // Persistent Singleton Setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps GameManager & GameOverCanvas alive across scenes
        }
        else
        {
            // Destroy duplicate managers when entering new scenes
            Destroy(gameObject);
            return;
        }

        ResetState();
    }

    void OnEnable()
    {
        // Subscribe to scene loads to automatically reset state on level changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Automatically hide the defeat screen whenever a new scene finishes loading
        ResetState();
    }

    private void ResetState()
    {
        gameOver = false;
        surviveTime = 0f;
        Time.timeScale = 1f;

        if (defeatPanel != null)
            defeatPanel.SetActive(false);
    }

    void Update()
    {
        if (!gameOver)
            surviveTime += Time.deltaTime;
    }

    public void ShowDefeat()
    {
        if (gameOver) return;
        gameOver = true;

        float best = PlayerPrefs.GetFloat("BestTime", 0f);
        if (surviveTime > best)
        {
            best = surviveTime;
            PlayerPrefs.SetFloat("BestTime", best);
            PlayerPrefs.Save();
        }

        if (timeText != null) timeText.text = FormatTime(surviveTime);
        if (bestTimeText != null) bestTimeText.text = "Best  " + FormatTime(best);

        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    string FormatTime(float t)
    {
        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t % 60f);
        return string.Format("{0}:{1:00}", m, s);
    }

    public void OnRestartClicked()
    {
        ResetState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenuClicked()
    {
        ResetState();
        SceneManager.LoadScene(menuSceneName);
    }
}