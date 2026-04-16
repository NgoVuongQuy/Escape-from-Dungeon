using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject gameOverMenu;
    public GameObject gamePauseMenu;
    public GameObject instructionMenu;

    private bool isPaused = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        EnsureEventSystem();
    }

    private void Start()
    {
        // Chỉ hiện main menu khi ở Scene1
        if (SceneManager.GetActiveScene().name == "Scene1")
        {
            ShowMainMenu();
        }
        else
        {
            HideAllMenus();
        }
    }

    private void Update()
    {
        // ESC để pause/resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Chỉ pause/resume khi không có menu nào khác đang hiện
            if (!IsAnyMenuActive())
            {
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
            }
        }
    }

    // Kiểm tra xem có menu nào đang active không (trừ pause menu)
    private bool IsAnyMenuActive()
    {
        return mainMenu.activeInHierarchy || gameOverMenu.activeInHierarchy || instructionMenu.activeInHierarchy;
    }

    private void HideAllMenus()
    {
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        gamePauseMenu.SetActive(false);
        instructionMenu.SetActive(false);
    }

    public void ShowMainMenu()
    {
        HideAllMenus();
        mainMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = false;
    }

    public void ShowGameOverMenu()
    {
        HideAllMenus();
        gameOverMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = false;
    }

    public void ShowPauseMenu()
    {
        HideAllMenus();
        gamePauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ShowInstructionMenu()
    {
        HideAllMenus();
        instructionMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        HideAllMenus();
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        ShowPauseMenu();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        HideAllMenus();

        // Restart logic
        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.RestartGame();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("Scene1");
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            DontDestroyOnLoad(eventSystemGO);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        // Đảm bảo EventSystem vẫn tồn tại sau khi load scene
        EnsureEventSystem();
    }

    public void StartGame()
    {
        ResumeGame(); // Tái sử dụng logic resume
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
