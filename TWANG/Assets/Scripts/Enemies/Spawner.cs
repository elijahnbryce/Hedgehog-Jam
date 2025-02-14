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
        public float weight; 
    }

    public Vector2 spawnBoundsMin = new Vector2(-9f, -6f);
    public Vector2 spawnBoundsMax = new Vector2(9f, 6f);
    public float spawnCheckRadius = 4f;
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
        doneSpawning = false;
        spawnLim = enemyAmt;
        intervalMod = (Mathf.Max(47f, (125f - (1.6f * spawnLim))) / 100f);
        Debug.Log($"Interval modifier: {intervalMod}");
        StartCoroutine(SpawnEnemy());
    }

    private Vector2 GetRandomSpawnPosition()
    {
        int maxAttempts = 30; 
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(spawnBoundsMin.x, spawnBoundsMax.x),
                Random.Range(spawnBoundsMin.y, spawnBoundsMax.y)
            );

            Collider2D[] overlaps = Physics2D.OverlapCircleAll(randomPos, spawnCheckRadius);
            bool isValidSpawn = true;

            foreach (Collider2D overlap in overlaps)
            {
                if (overlap.CompareTag("Enemy") || overlap.gameObject.layer == LayerMask.NameToLayer("Wall") || overlap.gameObject.layer == LayerMask.NameToLayer("Player"))
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
        return Vector2.zero; 
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

    private bool AreEnemiesPresent()
    {
        // Find all GameObjects with the "Enemy" tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        // Return true if there are any enemies, false otherwise
        return enemies.Length > 0;
    }

    private IEnumerator SpawnEnemy()
    {
        while (!doneSpawning)
        {
            EnemyType selectedEnemy = SelectRandomEnemyType();
            Vector2 spawnPosition = GetRandomSpawnPosition();

            //GameObject indicator = Instantiate(spawnIndicatorPrefab, spawnPosition, Quaternion.identity);
            //yield return new WaitForSeconds(1f); // Wait for indicator
            //Destroy(indicator);

            GameObject newEnemy = Instantiate(selectedEnemy.enemyPrefab, spawnPosition, Quaternion.identity);

            //this behavior is for the pencil enemy
            if (newEnemy.name.Contains("Parent"))
            {
                foreach(Transform child in newEnemy.transform)
                {
                    gm.AddEnemy(child.gameObject);
                }
            }
            else if (!newEnemy.name.Contains("Helper"))
                gm.AddEnemy(newEnemy);

            spawnLim--;
            doneSpawning = spawnLim <= 0;

            if (!doneSpawning)
            {
                // Only wait for the interval if there are enemies present
                if (AreEnemiesPresent())
                {
                    yield return new WaitForSeconds(selectedEnemy.spawnInterval * intervalMod);
                }
                else
                {
                    // If no enemies are present, continue immediately to the next spawn
                    yield return null;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            (spawnBoundsMin + spawnBoundsMax) / 2f,
            spawnBoundsMax - spawnBoundsMin
        );
    }
}