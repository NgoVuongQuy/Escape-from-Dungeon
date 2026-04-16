using UnityEngine;

public class BossManager : MonoBehaviour
{
    [Header("Boss Battle Settings")]
    public GameObject[] ghosts;           // Mảng chứa tất cả Ghost trong scene
    public GameObject portal;             // Portal object cần hiển thị
    public GameObject portalVFX;          // VFX khi portal xuất hiện (optional)
    
    private int totalGhosts;
    private int ghostsDefeated = 0;
    private bool portalActivated = false;
    
    void Start()
    {
        // Tự động tìm tất cả Ghost trong scene nếu chưa gán
        if (ghosts == null || ghosts.Length == 0)
        {
            GameObject[] foundGhosts = GameObject.FindGameObjectsWithTag("Enemy");
            ghosts = foundGhosts;
        }
        
        totalGhosts = ghosts.Length;
        
        // Ẩn portal ban đầu
        if (portal != null)
        {
            portal.SetActive(false);
        }
    }
    
    void OnEnable()
    {
        // Đăng ký event từ EnemyHealth
        EnemyHealth.OnEnemyDefeated += OnGhostDefeated;
    }
    
    void OnDisable()
    {
        // Hủy đăng ký event
        EnemyHealth.OnEnemyDefeated -= OnGhostDefeated;
    }

    private void OnGhostDefeated()
    {
        if (portalActivated) return;

        ghostsDefeated++;

        // Kiểm tra nếu tất cả Ghost đã bị tiêu diệt
        if (ghostsDefeated >= totalGhosts)
        {
            ActivatePortal();
        }
        
        if (portal != null)
    {
        Portal portalScript = portal.GetComponent<Portal>();
        if (portalScript != null)
        {
            portalScript.ActivatePortal();
        }
        else
        {
            portal.SetActive(true);
        }
    }
    }
    
    private void ActivatePortal()
    {
        if (portalActivated) return;
        
        portalActivated = true;
        
        // Hiển thị portal
        if (portal != null)
        {
            portal.SetActive(true);
        }
        
        // Spawn VFX 
        if (portalVFX != null && portal != null)
        {
            Instantiate(portalVFX, portal.transform.position, Quaternion.identity);
        }
    }
    
    private System.Collections.IEnumerator PortalAppearEffect()
    {
        if (portal == null) yield break;
        
        // Hiệu ứng scale từ 0 lên 1
        Transform portalTransform = portal.transform;
        Vector3 originalScale = portalTransform.localScale;
        portalTransform.localScale = Vector3.zero;
        
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            // Sử dụng easing curve để có hiệu ứng mượt mà hơn
            float easedProgress = Mathf.Sin(progress * Mathf.PI * 0.5f);
            portalTransform.localScale = Vector3.Lerp(Vector3.zero, originalScale, easedProgress);
            
            yield return null;
        }
        
        portalTransform.localScale = originalScale;
    }
    
    // Method để kiểm tra trạng thái boss battle
    public bool IsBossDefeated()
    {
        return portalActivated;
    }
    
    public int GetRemainingGhosts()
    {
        return totalGhosts - ghostsDefeated;
    }
}
