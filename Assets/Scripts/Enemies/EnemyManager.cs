using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public List<GameObject> enemies = new List<GameObject>();
    public GameObject portal;

    [Header("Events")]
    public static System.Action OnAllEnemiesDefeated;

    private int totalEnemies;
    private int defeatedEnemies = 0;

    void Start()
    {
        // Đếm tổng số quái vật
        totalEnemies = enemies.Count;

        // Ẩn cổng ban đầu
        if (portal != null)
            portal.SetActive(false);

        // Đăng ký sự kiện khi quái vật bị tiêu diệt
        EnemyHealth.OnEnemyDefeated += OnEnemyDefeated;

        // Đợi một frame để EnemySpawner có thể add enemies vào list
        Invoke(nameof(UpdateTotalEnemyCount), 0.1f);
    }

    void OnDestroy()
    {
        // Hủy đăng ký sự kiện
        EnemyHealth.OnEnemyDefeated -= OnEnemyDefeated;
    }

    private void UpdateTotalEnemyCount()
    {
        totalEnemies = enemies.Count;
        Debug.Log($"Total enemies to defeat: {totalEnemies}");
    }

    private void OnEnemyDefeated()
    {
        defeatedEnemies++;

        enemies.RemoveAll(enemy => enemy == null);

        Debug.Log($"Enemy defeated! Remaining: {enemies.Count}/{totalEnemies}");

        // Kiểm tra xem đã tiêu diệt hết quái chưa
        if (defeatedEnemies >= totalEnemies)
        {
            ShowPortal();
            OnAllEnemiesDefeated?.Invoke();
        }
    }

    private void ShowPortal()
    {
        if (portal != null)
        {
            portal.SetActive(true);
            Debug.Log("Cổng đã xuất hiện!");

            // Có thể thêm hiệu ứng xuất hiện
            StartCoroutine(PortalAppearEffect());
        }
    }

    private System.Collections.IEnumerator PortalAppearEffect()
    {
        // Hiệu ứng xuất hiện từ từ
        if (portal.TryGetComponent<Renderer>(out Renderer renderer))
        {
            Color color = renderer.material.color;
            color.a = 0f;
            renderer.material.color = color;

            float duration = 1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Lerp(0f, 1f, elapsed / duration);
                renderer.material.color = color;
                yield return null;
            }
        }
    }

    // Phương thức để thêm quái vật mới vào danh sách
    public void AddEnemy(GameObject enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            totalEnemies++;
        }
    }

    // Phương thức để kiểm tra trạng thái
    public bool AllEnemiesDefeated()
    {
        return defeatedEnemies >= totalEnemies;
    }
    
    public int GetRemainingEnemyCount()
    {
        return totalEnemies - defeatedEnemies;
    }
    
    public int GetDefeatedEnemyCount()
    {
        return defeatedEnemies;
    }
    
    public int GetTotalEnemyCount()
    {
        return totalEnemies;
    }
}
