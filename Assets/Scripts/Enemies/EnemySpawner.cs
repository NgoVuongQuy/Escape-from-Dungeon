using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] private int enemyCount = 5;
    [SerializeField] private float minDistanceFromPlayer = 5f;
    [SerializeField] private float minDistanceBetweenEnemies = 2f;
    
    [Header("Spawn Area")]
    [SerializeField] private Transform spawnAreaCenter;
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(20f, 20f);
    [SerializeField] private bool useSpawnPoints = false;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    
    private EnemyManager enemyManager;
    private List<Vector2> usedPositions = new List<Vector2>();
    
    private void Awake()
    {
        enemyManager = FindFirstObjectByType<EnemyManager>();
        if (spawnAreaCenter == null)
        {
            spawnAreaCenter = transform;
        }
    }
    
    private void Start()
    {
        SpawnAllEnemies();
    }
    
    private void SpawnAllEnemies()
    {
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("Không có enemy prefab nào được gán!");
            return;
        }
        
        usedPositions.Clear();
        
        for (int i = 0; i < enemyCount; i++)
        {
            // Chọn random enemy prefab
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            
            // Tìm vị trí spawn hợp lệ
            Vector2 spawnPosition = GetValidSpawnPosition();
            
            // Spawn enemy
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            
            // Thêm vào EnemyManager nếu có
            if (enemyManager != null)
            {
                enemyManager.AddEnemy(newEnemy);
            }
            
            // Lưu vị trí đã sử dụng
            usedPositions.Add(spawnPosition);
            
            Debug.Log($"Spawned {enemyPrefab.name} at {spawnPosition}");
        }
        
        Debug.Log($"Đã spawn {enemyCount} enemies!");
    }
    
    private Vector2 GetValidSpawnPosition()
    {
        Vector2 spawnPos = Vector2.zero;
        int maxAttempts = 50;
        int attempts = 0;
        
        if (useSpawnPoints && spawnPoints.Count > 0)
        {
            // Sử dụng spawn points được định sẵn
            List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);
            
            // Loại bỏ những spawn point đã được sử dụng
            for (int i = availableSpawnPoints.Count - 1; i >= 0; i--)
            {
                Vector2 pointPos = availableSpawnPoints[i].position;
                if (IsPositionUsed(pointPos))
                {
                    availableSpawnPoints.RemoveAt(i);
                }
            }
            
            if (availableSpawnPoints.Count > 0)
            {
                Transform randomSpawnPoint = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
                return randomSpawnPoint.position;
            }
        }
        
        // Tìm vị trí random trong khu vực spawn
        do
        {
            attempts++;
            
            // Random position trong spawn area
            float randomX = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
            float randomY = Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);
            
            spawnPos = (Vector2)spawnAreaCenter.position + new Vector2(randomX, randomY);
            
        } while (attempts < maxAttempts && !IsValidSpawnPosition(spawnPos));
        
        return spawnPos;
    }
    
    private bool IsValidSpawnPosition(Vector2 position)
    {
        // Kiểm tra khoảng cách với player
        if (Player.Instance != null)
        {
            float distanceToPlayer = Vector2.Distance(position, Player.Instance.transform.position);
            if (distanceToPlayer < minDistanceFromPlayer)
            {
                return false;
            }
        }
        
        // Kiểm tra khoảng cách với các enemies đã spawn
        foreach (Vector2 usedPos in usedPositions)
        {
            float distanceToUsedPos = Vector2.Distance(position, usedPos);
            if (distanceToUsedPos < minDistanceBetweenEnemies)
            {
                return false;
            }
        }
        
        // Kiểm tra va chạm với các object khác (optional)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall") || collider.CompareTag("Obstacle"))
            {
                return false;
            }
        }
        
        return true;
    }
    
    private bool IsPositionUsed(Vector2 position)
    {
        foreach (Vector2 usedPos in usedPositions)
        {
            if (Vector2.Distance(position, usedPos) < minDistanceBetweenEnemies)
            {
                return true;
            }
        }
        return false;
    }
    
    // Method để spawn thêm enemies nếu cần (có thể gọi từ script khác)
    public void SpawnAdditionalEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            Vector2 spawnPosition = GetValidSpawnPosition();
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            
            if (enemyManager != null)
            {
                enemyManager.AddEnemy(newEnemy);
            }
            
            usedPositions.Add(spawnPosition);
        }
    }
    
    // Visualize spawn area trong Scene view
    private void OnDrawGizmosSelected()
    {
        if (spawnAreaCenter != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(spawnAreaCenter.position, spawnAreaSize);
            
            Gizmos.color = Color.red;
            if (Player.Instance != null)
            {
                Gizmos.DrawWireSphere(Player.Instance.transform.position, minDistanceFromPlayer);
            }
        }
        
        if (useSpawnPoints && spawnPoints.Count > 0)
        {
            Gizmos.color = Color.green;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                }
            }
        }
        
        // Hiển thị vị trí đã spawn (trong Play mode)
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            foreach (Vector2 usedPos in usedPositions)
            {
                Gizmos.DrawWireSphere(usedPos, 0.3f);
            }
        }
    }
}