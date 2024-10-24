using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// handles the spawning and placement of erasers in a grid-based system
/// ensures proper spacing and positioning of erasers without overlap
/// </summary>
public class EraserSpawner : MonoBehaviour
{
    // constants for spawn configuration
    private const int DEFAULT_MAX_SPAWN_ATTEMPTS = 100;

    // grid dimensions
    private const int GRID_WIDTH = 20;
    private const int GRID_HEIGHT = 10;

    // serialized fields for configuration
    [SerializeField] private List<GameObject> eraserPrefabs;
    [SerializeField] private int numberOfErasers = 10;

    // internal state
    private bool[,] occupiedGrid;

    /// <summary>
    /// initializes the grid and starts the eraser spawning process
    /// </summary>
    private void Start()
    {
        InitializeGrid();
        SpawnErasers();
    }

    /// <summary>
    /// initializes the occupation grid with default values
    /// </summary>
    private void InitializeGrid()
    {
        occupiedGrid = new bool[GRID_WIDTH, GRID_HEIGHT];
    }

    /// <summary>
    /// main spawning logic for placing erasers in the grid
    /// </summary>
    private void SpawnErasers()
    {
        int successfulSpawns = 0;

        while (successfulSpawns < numberOfErasers)
        {
            if (TrySpawnEraser(successfulSpawns))
            {
                successfulSpawns++;
            }
            else
            {
                LogSpawnFailure();
                break;
            }
        }
    }

    /// <summary>
    /// attempts to spawn a single eraser in the grid
    /// </summary>
    /// <param name="spawnIndex">current spawn attempt index</param>
    /// <returns>true if spawn was successful, false otherwise</returns>
    private bool TrySpawnEraser(int spawnIndex)
    {
        GameObject eraserInstance = CreateEraserInstance();
        EraserSpawnData spawnData = GetEraserSpawnData(eraserInstance);

        Vector2Int? validPosition = FindValidSpawnPosition(spawnData);

        if (!validPosition.HasValue)
        {
            Destroy(eraserInstance);
            return false;
        }

        FinalizeEraserSpawn(eraserInstance, spawnData, validPosition.Value);
        return true;
    }

    /// <summary>
    /// creates a new instance of a random eraser prefab
    /// </summary>
    private GameObject CreateEraserInstance()
    {
        GameObject randomPrefab = eraserPrefabs[Random.Range(0, eraserPrefabs.Count)];
        return Instantiate(randomPrefab);
    }

    /// <summary>
    /// extracts spawn-relevant data from an eraser instance
    /// </summary>
    private EraserSpawnData GetEraserSpawnData(GameObject eraserInstance)
    {
        Eraser eraser = eraserInstance.GetComponent<Eraser>();
        Vector2 rawSize = eraser.Size;

        return new EraserSpawnData
        {
            Width = Mathf.CeilToInt(rawSize.x),
            Height = Mathf.CeilToInt(rawSize.y)
        };
    }

    /// <summary>
    /// attempts to find a valid position for spawning the eraser
    /// </summary>
    private Vector2Int? FindValidSpawnPosition(EraserSpawnData spawnData)
    {
        for (int attempt = 0; attempt < DEFAULT_MAX_SPAWN_ATTEMPTS; attempt++)
        {
            Vector2Int testPosition = GenerateRandomPosition(spawnData);

            if (IsSpaceAvailable(testPosition.x, testPosition.y, spawnData.Width, spawnData.Height))
            {
                return testPosition;
            }
        }

        return null;
    }

    /// <summary>
    /// generates a random position within the valid grid bounds for the given eraser size
    /// </summary>
    private Vector2Int GenerateRandomPosition(EraserSpawnData spawnData)
    {
        int x = Random.Range(0, GRID_WIDTH - spawnData.Width + 1);
        int y = Random.Range(0, GRID_HEIGHT - spawnData.Height + 1);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// finalizes the eraser spawn by setting position and marking grid space
    /// </summary>
    private void FinalizeEraserSpawn(GameObject eraserInstance, EraserSpawnData spawnData, Vector2Int position)
    {
        MarkSpaceAsOccupied(position.x, position.y, spawnData.Width, spawnData.Height);

        Vector3 spawnPosition = new Vector3(
            position.x + spawnData.Width / 2f,
            position.y + spawnData.Height / 2f,
            0
        );

        eraserInstance.transform.position = spawnPosition;
    }

    /// <summary>
    /// checks if a given space in the grid is available for an eraser
    /// </summary>
    private bool IsSpaceAvailable(int startX, int startY, int width, int height)
    {
        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                if (occupiedGrid[x, y])
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// marks a space in the grid as occupied by an eraser
    /// </summary>
    private void MarkSpaceAsOccupied(int startX, int startY, int width, int height)
    {
        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                occupiedGrid[x, y] = true;
            }
        }
    }

    private void LogSpawnFailure()
    {
        Debug.LogWarning("No available space to fit the erasers.");
    }
}

/// <summary>
/// data structure to hold processed eraser spawn parameters
/// </summary>
public struct EraserSpawnData
{
    public int Width { get; set; }
    public int Height { get; set; }
}