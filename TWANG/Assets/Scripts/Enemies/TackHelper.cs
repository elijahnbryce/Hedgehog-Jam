using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TackHelper : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs = new();

    void Start()
    {
        float totalWeight = 0;
        for (int i = 0; i < prefabs.Count; i++)
        {
            totalWeight += 1f / Mathf.Pow(2, i);
        }

        float randomValue = Random.Range(0f, totalWeight);

        float currentWeight = 0;
        for (int i = 0; i < prefabs.Count; i++)
        {
            currentWeight += 1f / Mathf.Pow(2, i);
            if (randomValue <= currentWeight)
            {
                var newTack = Instantiate(prefabs[i], transform.position, Quaternion.identity);
                GameManager.Instance.AddEnemy(newTack);
                break;
            }
        }

        Destroy(gameObject);
    }
}
