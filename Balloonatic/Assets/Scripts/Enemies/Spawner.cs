using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField]
	private GameObject scissorEnemy;
	[SerializeField]
	private GameObject splittingEnemy;
	[SerializeField]
	private GameObject glueEnemy;

	[SerializeField]
	private float scissorInterval = 5f;
	[SerializeField]
	private float splittingInterval = 9f;
	[SerializeField]
	private float glueInterval = 12;

	[SerializeField]
	public Vector3[] spawnPoints;
    // Start is called before the first frame update
    void Start()
    {
       StartCoroutine(spawnEnemy(scissorInterval, scissorEnemy));
       StartCoroutine(spawnEnemy(splittingInterval, splittingEnemy)); 
       StartCoroutine(spawnEnemy(glueInterval, glueEnemy)); 
    }
	
    private IEnumerator spawnEnemy(float interval, GameObject enemy)
    {
	int randPoint = Random.Range(0, 3);

	yield return new WaitForSeconds(interval);
	GameObject newEnemy = Instantiate(enemy, spawnPoints[randPoint], Quaternion.identity);
	StartCoroutine(spawnEnemy(interval, enemy));
    }
}
