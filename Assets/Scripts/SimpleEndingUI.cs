using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimpleEndingUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text congratsText;
    public Button playAgainButton;
    public Button quitButton;

    const string TOWN_TEXT = "Scene1";

    void Start()
    {
        // Setup buttons
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Hiển thị text chúc mừng
        if (congratsText != null)
        {
            congratsText.text = "Chúc mừng!\nBạn đã hoàn thành game!";
        }
    }

    public void PlayAgain()
    {

        StartCoroutine(PlayAgainLoadSceneRoutine());
    }

    private IEnumerator PlayAgainLoadSceneRoutine()
    {
        // Có thể thêm một chút delay nếu cần
        yield return new WaitForSeconds(0.5f);

        // Destroy gameObject trước khi load scene
        Destroy(gameObject);

        // Load scene Town (Scene1)
        // SceneManager.LoadScene(TOWN_TEXT);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
