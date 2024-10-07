using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public static GameManager gm = GameManager.Instance;

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
	public Transform[] spawnPoints;
    // Start is called before the first frame update
    void Start()
    {
       StartCoroutine(SpawnEnemy(scissorInterval, scissorEnemy));
       StartCoroutine(SpawnEnemy(splittingInterval, splittingEnemy)); 
       StartCoroutine(SpawnEnemy(glueInterval, glueEnemy)); 
    }
	
    private IEnumerator SpawnEnemy(float interval, GameObject enemy)
    {
	int randPoint = Random.Range(0, 3);

	yield return new WaitForSeconds(interval);
	GameObject newEnemy = Instantiate(enemy, spawnPoints[randPoint].position, Quaternion.identity);

	//gm.AddEnemy(newEnemy);

	StartCoroutine(SpawnEnemy(interval, enemy));
    }

    //void SpawnEnemies(int enemyAmt)
    //{
    //    if (gm.AddEnemy > enemyAmt)
    //    	StopCoroutine(SpawnEnemy(interval, 	
    //}

}
