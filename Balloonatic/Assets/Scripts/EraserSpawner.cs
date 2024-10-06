using System.Collections.Generic;
using UnityEngine;

public class EraserSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> eraserPrefabs; // List of Eraser prefabs
    [SerializeField] private int numberOfErasers = 10; // Number of erasers to spawn

    private int boxWidth = 20;
    private int boxHeight = 10;

    // 2D grid to track occupied cells
    private bool[,] occupiedGrid;

    private void Start()
    {
        // Initialize the grid (false means unoccupied)
        occupiedGrid = new bool[boxWidth, boxHeight];
        SpawnErasers();
    }

    private void SpawnErasers()
    {
        int spawnedErasers = 0;

        while (spawnedErasers < numberOfErasers)
        {
            // Pick a random eraser prefab from the list
            GameObject randomEraserPrefab = eraserPrefabs[Random.Range(0, eraserPrefabs.Count)];

            // Instantiate the eraser prefab
            GameObject eraserInstance = Instantiate(randomEraserPrefab);

            // Get the Eraser component to access its size
            Eraser eraser = eraserInstance.GetComponent<Eraser>();
            Vector2 eraserSize = eraser.Size;

            // Ensure that the eraser fits within the grid (size.x, size.y should be integers)
            int eraserWidth = Mathf.CeilToInt(eraserSize.x);
            int eraserHeight = Mathf.CeilToInt(eraserSize.y);

            // Find a random position that fits the eraser and is not occupied
            bool foundSpot = false;
            int startX = 0, startY = 0;

            for (int attempt = 0; attempt < 100; attempt++) // Try 100 times to find a valid spot
            {
                // Random position for the bottom-left corner of the eraser
                startX = Random.Range(0, boxWidth - eraserWidth + 1);
                startY = Random.Range(0, boxHeight - eraserHeight + 1);

                if (IsSpaceAvailable(startX, startY, eraserWidth, eraserHeight))
                {
                    foundSpot = true;
                    break;
                }
            }

            if (!foundSpot)
            {
                Debug.LogWarning("No available space to fit the erasers.");
                break;
            }

            // Mark the grid space as occupied
            MarkSpaceAsOccupied(startX, startY, eraserWidth, eraserHeight);

            // Set the position of the eraser within the box (centered in its grid space)
            eraserInstance.transform.position = new Vector3(startX + eraserWidth / 2f, startY + eraserHeight / 2f, 0);

            spawnedErasers++;
        }
    }

    // Check if the space is available on the grid for the eraser
    private bool IsSpaceAvailable(int startX, int startY, int width, int height)
    {
        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                if (occupiedGrid[x, y])
                {
                    return false; // Space is already occupied
                }
            }
        }
        return true; // Space is available
    }

    // Mark the grid space as occupied
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
}
