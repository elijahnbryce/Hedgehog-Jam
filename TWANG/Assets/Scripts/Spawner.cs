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

    private int spawnLim = 0;

    private float intervalMod = 0f;
		
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
        spawnLim = enemyAmt;
        intervalMod = (Mathf.Max(47f, (125f - (1.6f * spawnLim))) / 100f);
        Debug.Log(intervalMod);
        StartCoroutine(SpawnEnemy(enemyTypes));
    } 
	
    private IEnumerator SpawnEnemy(int typeLim, int toSpawn = 1)
    {
        int randPoint = -1;

        for (int i = 0, temp = randPoint; i < toSpawn; i++)
        {
            enemySpwnProp enemy = enemyStructList[Random.Range(0, typeLim)];

            //Debug.Log(Time.timeSinceLevelLoad + " SpawnEnemy: " + typeLim + enemy.enemyType.name); //System.DateTime.Now.TimeOfDay
            temp = randPoint;
            while (randPoint == temp) { randPoint = Random.Range(0, 3); }

            //enemySpwnProp enemy = enemyStructList[Random.Range(0, typeLim)];
	        yield return new WaitForSeconds(enemy.typeInterval * intervalMod);
	        GameObject newEnemy = Instantiate(enemy.enemyType, new Vector2(Random.Range(-9, 9), Random.Range(-6, 6)), Quaternion.identity);
	        gm.AddEnemy(newEnemy);
            spawnLim--;
            doneSpawning = spawnLim <= 0;
            //Debug.Log(spawnLim + " donespawning:" + doneSpawning);

            if (!doneSpawning)
                StartCoroutine(SpawnEnemy(typeLim)); // always 1 toSpawn
        }
    } 
}
