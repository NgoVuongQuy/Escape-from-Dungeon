using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Canvas Settings")]
    public GameObject gameCanvas;      // Canvas chính của game
    public GameObject endingCanvas;    // Canvas ending

    [SerializeField] private Transform player;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private GameObject hintUI;

    private bool hasUsedPortal = false;
    private bool isPortalActive = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Setup canvas
        if (endingCanvas != null)
            endingCanvas.SetActive(false);

        if (gameCanvas != null)
            gameCanvas.SetActive(true);

        // Portal bắt đầu ở trạng thái không hoạt động
        SetPortalActive(false);
    }

    void Awake()
    {
        hintUI.SetActive(false); // tắt hint lúc đầu
    }


    void Update()
    {
        float distance = Vector2.Distance(player.position, transform.position);

        if (distance <= interactionDistance && !hasUsedPortal)
        {
            hintUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                GoToEnding();
            }
        }
        else
        {
            hintUI.SetActive(false);
        }
    }

    public void ActivatePortal()
    {
        if (isPortalActive) return;

        SetPortalActive(true);
    }

    private void SetPortalActive(bool active)
    {
        isPortalActive = active;
    }


    void GoToEnding()
    {
        hasUsedPortal = true;
        
        // Ẩn game canvas
        if (gameCanvas != null)
        {
            gameCanvas.SetActive(false);
        }
        
        // Hiển thị ending canvas
        if (endingCanvas != null)
        {
            endingCanvas.SetActive(true);
        }
        
        // Optional: Dừng game hoặc disable player movement
        // Time.timeScale = 0f; // Dừng game
        
        // Hoặc disable player movement
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Disable player movement script nếu có
            var playerMovement = player.GetComponent<MonoBehaviour>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
        }
    }
    
    // Method để restart game nếu cần (có thể gọi từ ending canvas)
    public void RestartGame()
    {
        hasUsedPortal = false;
        Time.timeScale = 1f;
        
        if (gameCanvas != null)
            gameCanvas.SetActive(true);
            
        if (endingCanvas != null)
            endingCanvas.SetActive(false);
            
        // Enable lại player movement
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var playerMovement = player.GetComponent<MonoBehaviour>();
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
        }
    }

    // Getter methods
    public bool IsPortalActive()
    {
        return isPortalActive;
    }

    public bool HasBeenUsed()
    {
        return hasUsedPortal;
    }
}