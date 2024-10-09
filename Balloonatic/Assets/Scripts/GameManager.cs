using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private EventHandler eV;

    public Spawner sp = Spawner.Instance;

    [Header("Game Status")]
    [SerializeField] private static int fullHealth = 3;
    public int health = fullHealth, wave = 0, startEnemies = 0;
    public bool gameOver, gameActive, gamePaused;

    private bool betweenRounds = true;
    public bool BetweenRounds { get; set; }
    //?? fix later

    private int levelScore, totalScore = 0;

    public List<GameObject> enemyList = new List<GameObject>();
    public Dictionary<UpgradeType, int> upgradeList = new();
    //private Timer ts;
    private bool isInvicible = false;
    private float scoreMult = 1.1f;

    private Camera cam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Game Manager Already Exists");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Game Manager Set");
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        sp = Spawner.Instance;
        eV = GetComponent<EventHandler>();
        SetLevel();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            eV.PauseGame();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.visible = false;
        }
    }

    public void ReStart()
    {
        SetLevel(true);
    }

    public void Kill()
    {
        Instance = null;
        Debug.Log(Instance);
        Destroy(gameObject);
    }

    private void SetLevel(bool restart = false)
    {
        Debug.Log("SETTING LVL");
        cam = Camera.main;
        EverythingFalse();
        Time.timeScale = 1;
        //timer = 0

        UpdateScore(0);
        UpdateHealth(0);

        NewWave();
    }

    private void EverythingFalse()
    {
        eV.UIFalse();

        gameOver = false;
        gameActive = true;
        gamePaused = false;
    }
    public void NewWave()
    {
        //if (restart) return;
        Debug.Log("New wave");

        betweenRounds = false;
        EraserManager.Instance.SpawnConfig();

        wave++;
        int toSpawn = Mathf.FloorToInt(10 / wave) + 1;
        startEnemies = 3;

        sp.StartSpawn(3, 1);

        //if (wave == 1)
        //{
        //    sp.StartSpawn(startEnemies, 1);
        //}
        //else
        //{
        //    sp.StartSpawn(startEnemies, 1);
        //}
        //else if (wave == 2) {
        //	sp.StartSpawn(20, 1);
        //}

        // change walls or something
        // ? some effect for enemies
    }

    private int CalcLevelScore(int score)
    {
        Debug.Log("Score Calc");
        int newScore = score;
        //float timeElapsed = ts.GetTime();
        //float timeMult = (timeElapsed / 60 < 4) ? (1 - timeElapsed / 300) : 0.1f;

        //newScore = Mathf.RoundToInt(score * ((1 + (float)health / fullHealth) * timeMult) - (enemyFactor * enemies.Count) * makeScoreBigger);
        //  or multiply by (1 - enemies.Count / enemies2Spawn);                <- punishment
        // (1 + (float)(startEnemies - levEnemies.Count) / startEnemies)   <- reward
        return Mathf.Max(0, newScore);
    }

    public void EndLevel(bool alive, int score)
    {
        //Time.timeScale = 0;
        gameActive = false;

        totalScore += CalcLevelScore(score);

        if (alive)
        {
            // wave intermission
            Debug.Log("Wave Complete");
            //

            BetweenRounds = betweenRounds = true;
            UpgradeManager.Instance.SpawnUpgrades();
            //NewWave();
        }
        else
        {
            eV.LoseGame(totalScore);
        }
    }

    public bool getGameActive()
    {
        return (gameActive && !gamePaused);
    }

    public void AddEnemy(GameObject enemy)
    {
        enemyList.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        // add points
        enemyList.Remove(enemy);
        Destroy(enemy);

        if (upgradeList.ContainsKey(UpgradeType.Pizza))
        {
            if (health < fullHealth || ((float)(startEnemies - enemyList.Count) % 10f) == 0f) { UpdateHealth(1); }
        }

        if(sp.DoneSpawning && enemyList.Count == 0)
        {
            EndLevel(true, levelScore);
        }
    }

    public void AddPowerUP(UpgradeType upgrade)
    {
        if (!upgradeList.ContainsKey(upgrade))
        { upgradeList.Add(upgrade, 0); }
        upgradeList[upgrade]++;

        if (CheckInstanceConsume(upgrade)) DecPowerUp(upgrade);
    }

    public void DecPowerUp(UpgradeType upgrade)
    {
        if (upgradeList.ContainsKey(upgrade))
        {
            if (upgradeList[upgrade] > 0)
            { upgradeList[upgrade]--; }
            else
            {
                upgradeList.Remove(upgrade);
            }
        }
    }

    private bool CheckInstanceConsume(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Heart:
                UpdateHealth(1);
                return true;
            case UpgradeType.Star:
                StartCoroutine(StarPower());
                return true;
        }
        return false;
    }

    public float GetPowerMult(UpgradeType upgrade, float percent = 1.1f)
    {
        return (upgradeList.ContainsKey(upgrade)) ? Mathf.Pow(percent, upgradeList[upgrade]) : 1f; ;
    }

    //private int GetFinalScore(int score, float time = 10f)
    //{
    //    return Mathf.RoundToInt((1 + (float)score / (float)(fullHealth / 2)) * (1 - (1 - time / 10f)) * (1 + (float)(startEnemies - levEnemies.Count) / startEnemies) * ((float)(startPickups - levPickups.Count) / startPickups) * 1017); // score is just health
    //}

    private IEnumerator StarPower()
    {
        isInvicible = true;
        yield return new WaitForSeconds(10f * upgradeList[UpgradeType.Star]);
        isInvicible = false;
    }

    public void UpdateHealth(int change = -1)
    {
        if (isInvicible) { return; }
        if (change < 0)
        {
            SoundManager.Instance.PlaySoundEffect("player_damage");
        }
        health += change;
        eV.DisplayHealth(health);

        if (health <= 0)
        {
            health = 0;
            EndLevel(false, levelScore);
        }
    }

    public void UpdateScore(int change = 1)
    {
        levelScore += change;
        eV.DisplayScore(levelScore);
    }

    public void SetHealth(int val = 0)
    {
        health = (val == 0) ? fullHealth : val;
        UpdateHealth();
    }

    public void SetScore(int val)
    {
        levelScore = val;
        UpdateScore();
    }

    public float GetHealthRatio()
    {
        return (float)health / fullHealth;
    }
}
