using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; private set; }
    public GameManager gm = GameManager.Instance;
    private bool doneSpawning;
    public bool DoneSpawning { get { return doneSpawning; } }
    private int spawnLim = 0;
    private float intervalMod = 0f;

    [System.Serializable]
    public struct EnemyType
    {
        public GameObject enemyPrefab;
        public float spawnInterval;
        public float weight; // Weight for random selection
    }

    public Vector2 spawnBoundsMin = new Vector2(-9f, -6f);
    public Vector2 spawnBoundsMax = new Vector2(9f, 6f);
    public float spawnCheckRadius = 4f;
    public GameObject spawnIndicatorPrefab;
    public List<EnemyType> enemyTypes = new List<EnemyType>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Spawner Already Exists");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Spawner Set");
            Instance = this;
        }
    }

    private void Start()
    {
        gm = GameManager.Instance;
    }

    public void StartSpawn(int enemyAmt)
    {
        spawnLim = enemyAmt;
        intervalMod = (Mathf.Max(47f, (125f - (1.6f * spawnLim))) / 100f);
        Debug.Log($"Interval modifier: {intervalMod}");
        StartCoroutine(SpawnEnemy());
    }

    private Vector2 GetRandomSpawnPosition()
    {
        int maxAttempts = 30; // Prevent infinite loops
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(spawnBoundsMin.x, spawnBoundsMax.x),
                Random.Range(spawnBoundsMin.y, spawnBoundsMax.y)
            );

            // Check for overlapping entities
            Collider2D[] overlaps = Physics2D.OverlapCircleAll(randomPos, spawnCheckRadius);
            bool isValidSpawn = true;

            foreach (Collider2D overlap in overlaps)
            {
                if (overlap.CompareTag("Enemy") || overlap.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    isValidSpawn = false;
                    break;
                }
            }

            if (isValidSpawn)
            {
                return randomPos;
            }

            attempts++;
        }

        Debug.LogWarning("Could not find valid spawn position after " + maxAttempts + " attempts");
        return Vector2.zero; // Fallback position
    }

    private EnemyType SelectRandomEnemyType()
    {
        float totalWeight = 0;
        foreach (EnemyType enemy in enemyTypes)
        {
            totalWeight += enemy.weight;
        }

        float random = Random.Range(0f, totalWeight);
        float weightSum = 0;

        foreach (EnemyType enemy in enemyTypes)
        {
            weightSum += enemy.weight;
            if (random <= weightSum)
            {
                return enemy;
            }
        }

        return enemyTypes[0]; // Fallback to first enemy type
    }

    private IEnumerator SpawnEnemy()
    {
        while (!doneSpawning)
        {
            EnemyType selectedEnemy = SelectRandomEnemyType();
            Vector2 spawnPosition = GetRandomSpawnPosition();

            // Spawn indicator
            GameObject indicator = Instantiate(spawnIndicatorPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(1f); // Wait for indicator
            Destroy(indicator);

            // Spawn enemy
            GameObject newEnemy = Instantiate(selectedEnemy.enemyPrefab, spawnPosition, Quaternion.identity);
            gm.AddEnemy(newEnemy);

            spawnLim--;
            doneSpawning = spawnLim <= 0;

            if (!doneSpawning)
            {
                yield return new WaitForSeconds(selectedEnemy.spawnInterval * intervalMod);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw spawn bounds in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            (spawnBoundsMin + spawnBoundsMax) / 2f,
            spawnBoundsMax - spawnBoundsMin
        );
    }
}