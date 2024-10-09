using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public static Spawner Instance { get; private set; }

	public GameManager gm = GameManager.Instance;

    private bool doneSpawning;
    public bool DoneSpawning { get { return doneSpawning; } }

    private int toSpawn = 0;
		
	[System.Serializable] public struct enemySpwnProp
	{
		//public int enemyAmt;
		public float typeInterval;
		public GameObject enemyType;
	} 

	public List<enemySpwnProp> enemyStructList = new List<enemySpwnProp>();
		
	public Transform[] spawnPoints;
    // Start is called before the first frame update
       
    public void Awake()
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
            //DontDestroyOnLoad(gameObject);
        }
    }

    public void Start()
    {
	gm = GameManager.Instance;
    }

    public void StartSpawn(int enemyAmt, int enemyTypes)
    {
        toSpawn = enemyAmt;
	    StartCoroutine(SpawnEnemy(enemyAmt, enemyTypes));
    } 
	
    private IEnumerator SpawnEnemy(int spawnLim, int typeLim)
    {
        int randPoint = -1;

        for (int i = 0, temp; i < toSpawn; i++)
        {
            temp = randPoint;
            while (randPoint == temp) { randPoint = Random.Range(0, 3); }

	        enemySpwnProp enemy = enemyStructList[Random.Range(0, typeLim)];

	        yield return new WaitForSeconds(enemy.typeInterval);
	        GameObject newEnemy = Instantiate(enemy.enemyType, spawnPoints[randPoint].position, Quaternion.identity);
	        gm.AddEnemy(newEnemy);
            toSpawn--;
            doneSpawning = toSpawn < 0;
            Debug.Log("donespawning:" + doneSpawning);


            if (!doneSpawning)
                StartCoroutine(SpawnEnemy(spawnLim, typeLim)); // always 1 toSpawn
        }
    } 
}
