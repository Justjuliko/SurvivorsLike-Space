using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Prefabs of the enemies to spawn. Add them in the Inspector.")]
    public List<GameObject> enemyPrefabs;

    [Tooltip("Reference to the player object.")]
    public Transform player;

    [Header("Spawn Settings")]
    [Tooltip("Initial spawn interval in seconds.")]
    public float initialSpawnInterval = 5f;

    [Tooltip("Minimum spawn interval as the game progresses.")]
    public float minimumSpawnInterval = 1f;

    [Tooltip("Time (in seconds) it takes for the spawn interval to reach its minimum.")]
    public float spawnAccelerationTime = 60f;

    [Tooltip("Minimum distance from the player where enemies can spawn.")]
    public float spawnDistanceMin = 5f;

    [Tooltip("Maximum distance from the player where enemies can spawn.")]
    public float spawnDistanceMax = 10f;

    [Tooltip("Number of enemies to pool.")]
    public int poolSize = 20;

    private Queue<GameObject> enemyPool;
    private float currentSpawnInterval;
    private float spawnTimer;

    private void Start()
    {
        // Initialize the spawn interval
        currentSpawnInterval = initialSpawnInterval;

        // Create the enemy pool
        InitializeEnemyPool();

        // Start spawning enemies
        StartCoroutine(SpawnEnemies());
    }

    private void InitializeEnemyPool()
    {
        enemyPool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            // Randomly select an enemy prefab
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            // Instantiate the enemy and set it as a child of this spawner
            GameObject enemy = Instantiate(enemyPrefab, transform);
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy);
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // Spawn an enemy
            SpawnEnemy();

            // Wait for the current spawn interval
            yield return new WaitForSeconds(currentSpawnInterval);

            // Decrease the spawn interval over time
            currentSpawnInterval = Mathf.Max(
                minimumSpawnInterval,
                Mathf.Lerp(initialSpawnInterval, minimumSpawnInterval, spawnTimer / spawnAccelerationTime)
            );

            // Increment the timer
            spawnTimer += currentSpawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPool.Count > 0)
        {
            // Get an enemy from the pool
            GameObject enemy = enemyPool.Dequeue();

            // Activate and position the enemy
            enemy.SetActive(true);
            enemy.transform.position = GetRandomSpawnPosition();

            // Return the enemy to the pool when deactivated
            enemy.GetComponent<Enemy>().OnEnemyDeactivate += ReturnEnemyToPool;
        }
        else
        {
            Debug.LogWarning("No available enemies in the pool!");
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Calculate spawn position based on the player's current position
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(spawnDistanceMin, spawnDistanceMax);
        Vector3 spawnPosition = player.position + (Vector3)(randomDirection * randomDistance);
        return spawnPosition;
    }

    private void ReturnEnemyToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }
}
