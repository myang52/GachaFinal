using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab; // Enemy prefab
    [SerializeField]
    private float spawnRadius = 20f; // Spawn radius (20 feet = ~6.1 meters)
    [SerializeField]
    private float spawnInterval = 10f; // Time between spawns
    [SerializeField]
    private int maxEnemies = 10; // Max number of enemies in the level
    [SerializeField]
    private float navMeshSampleDistance = 2f; // NavMesh sample distance

    private Transform playerTransform;
    private int enemyCount = 0;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        InvokeRepeating("SpawnEnemy", spawnInterval, spawnInterval); // Start spawning enemies
    }

    private void SpawnEnemy()
    {
        if (playerTransform == null || enemyPrefab == null || enemyCount >= maxEnemies)
        {
            return;
        }

        // Generate a random position within the spawn radius
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;

        // Flatten the spawn position to the player's XZ plane
        randomDirection.y = 0;

        // Add the player's position to the random direction
        Vector3 spawnPosition = playerTransform.position + randomDirection;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            Instantiate(enemyPrefab, hit.position, Quaternion.identity); // Spawn enemy at valid NavMesh location
            enemyCount++;
        }
    }

    public void EnemyDestroyed()
    {
        enemyCount--;
    }
}
