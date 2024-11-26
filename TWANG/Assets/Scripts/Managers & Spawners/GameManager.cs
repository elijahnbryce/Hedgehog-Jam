using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private EventHandler eV;
    private Spawner sp;

    [Header("Game Status")]
    [SerializeField] private static int fullHealth = 3;
    public int health = fullHealth, wave = 0, startEnemies = 0, enemiesKilled = 0, enemyTypes;
    public bool gameOver, waveStarted;

    public bool GameRunning => (waveStarted && !GameUI.Instance.IsGamePaused && !gameOver);
    public bool BetweenRounds { get; set; }

    private float levelScore, totalScore = 0;

    public List<GameObject> enemyList = new List<GameObject>();
    public Dictionary<UpgradeType, int> upgradeList = new();
    private Timer ts;
    private bool isInvicible = false;
    [SerializeField] private static float multBonus = 1.2f;
    [SerializeField] private float invincibilityDuration = 2f;

    [Header("Effect Settings")]
    [SerializeField] private float flickerInterval = 0.1f;
    [SerializeField] private float slowMotionDuration = 0.05f;
    [SerializeField] private float slowMotionTimeScale = 0.25f;
    private float baseTimeScale = 1f;
    private bool inSlowMotion = false;

    [SerializeField] Vector2 spawnBoundsMin = new Vector2(-9f, -6f);
    [SerializeField] Vector2 spawnBoundsMax = new Vector2(9f, 6f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Game Manager Already Exists");
            Destroy(gameObject);
            return;
        }
        else
        {
            Debug.Log("Game Manager Set");
            Instance = this;
        }
        eV = GetComponent<EventHandler>();
        ts = GetComponent<Timer>();
        baseTimeScale = Time.timeScale;
        DOTween.SetTweensCapacity(500, 50);
        Application.targetFrameRate = 45;
        QualitySettings.vSyncCount = 0;
    }

    private void Start()
    {
        sp = Spawner.Instance;
        SetLevel();
    }

    private void Update()
    {
        if (!gameOver
            && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)))
        {
            GameUI.ToggleGamePause?.Invoke();
        }
    }
    public void Kill()
    {
        Instance = null;
        Debug.Log(Instance);
        Destroy(gameObject);
    }

    private void SetLevel()
    {
        Debug.Log("SETTING LVL");
        EverythingFalse();
        Time.timeScale = 1;

        UpdateScore(0);
        UpdateHealth(0);

        NewWave();
    }

    public List<Vector2> GetSpawnBounds()
    {
        return new List<Vector2> { spawnBoundsMin, spawnBoundsMax };
    }

    public void SetTimeScale(float scale)
    {
        if (!inSlowMotion)
        {
            baseTimeScale = scale;
            Time.timeScale = scale;
        }
    }

    private void EverythingFalse()
    {
        if (eV != null)
            eV.UIFalse();
        if(ts != null)
            ts.TimerStart();

        gameOver = false;
        GameUI.SetPauseState(false);
    }

    public void NewWave()
    {
        Debug.Log("New wave");
        ts.StartTime();
        Time.timeScale = 1;
        BetweenRounds = false;

        if (EraserManager.Instance != null)
            EraserManager.Instance.SpawnConfig();

        wave++;

        startEnemies = 1 + (wave * 2);
        Debug.Log("startenemies:" + startEnemies);
        enemyTypes = Mathf.Min(3, Mathf.CeilToInt(wave / 3f) + 1);
        //sp.StartSpawn(startEnemies, enemyTypes);
        sp.StartSpawn(startEnemies);
        waveStarted = true;
    }

    private float CalcLevelScore(float score)
    {
        Debug.Log("Score Calc");
        float newScore = score;
        //float timeElapsed = ts.GetTime();
        //float timeMult = (timeElapsed / 60 < 4) ? (1 - timeElapsed / 300) : 0.1f;
        float timeMult = 1f;
        float makeScoreBigger = 1f;

        float healthBonus = 1 + (GetHealthRatio() / 2f);

        newScore = score * (healthBonus * timeMult) * makeScoreBigger;
        //  or multiply by (1 - enemies.Count / enemies2Spawn);                <- punishment
        // (1 + (float)(startEnemies - levEnemies.Count) / startEnemies)   <- reward
        return Mathf.Max(0, newScore);
    }
    private int GetGuideScore()
    {
        float pointsFromEnemies = 0f;

        for (int i = 1; i <= wave; i++)
        {
            pointsFromEnemies += 16 * (3 + 2 * i);
            //pointsFromEnemies *= Matf.Pow(multBonus, Mathf.FloorToInt(10 / wave) + 1);
        }
        return Mathf.RoundToInt(1.47f * pointsFromEnemies);
    }

    public void EndLevel(bool alive, float score)
    {
        Debug.Log("End Level");
        //Time.timeScale = 0;
        waveStarted = false;
        ts.StopTime();

        totalScore += CalcLevelScore(score);

        if (alive)
        {
            // wave intermission
            Debug.Log("Wave Complete");
            Time.timeScale = 1;

            BetweenRounds = true;
            UpgradeManager.Instance.SpawnUpgrades();
            //NewWave();
        }
        else
        {
            Time.timeScale = 0;
            gameOver = true;
            eV.LoseGame(totalScore, GetGuideScore());
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (!enemyList.Contains(enemy))
        {
            Debug.LogWarning($"Trying to remove non-existent enemy: {enemy.name}");
            return;
        }

        enemiesKilled++;
        enemyList.Remove(enemy);

        Debug.Log($"Enemy removed: {enemy.name}. Remaining enemies: {enemyList.Count}. Done spawning: {sp.DoneSpawning}");

        // Only destroy if it hasn't been destroyed already
        if (enemy != null && enemy.gameObject != null)
        {
            Destroy(enemy);
        }

        if (sp.DoneSpawning && enemyList.Count == 0)
        {
            Debug.Log("All enemies eliminated - ending level");
            EndLevel(true, levelScore);
        }
    }

    public void AddEnemy(GameObject enemy)
    {
        if (enemyList.Contains(enemy))
        {
            Debug.LogWarning($"Trying to add duplicate enemy: {enemy.name}");
            return;
        }

        enemyList.Add(enemy);
        Debug.Log($"Enemy added: {enemy.name}. Total enemies: {enemyList.Count}");
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
                UpdateHealth(fullHealth - health);
                return true;
            case UpgradeType.Evil_Pizza:
                UpdateHealth();
                return true;
            case UpgradeType.Star:
                StartCoroutine(StarPower());
                return true;
            case UpgradeType.Confusion:
                return true;
        }
        return false;
    }

    public float GetPowerMult(UpgradeType upgrade, float percent = 1.1f)
    {
        return (upgradeList.ContainsKey(upgrade)) ? Mathf.Pow(percent, upgradeList[upgrade]) : 1f; ;
    }

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
            StartCoroutine(InvincibilityFrames());
            StartSlowMotionEffect();
        }

        health += change;
        eV.DisplayHealth(health);

        if (health <= 0)
        {
            health = 0;
            EndLevel(false, levelScore);
        }
    }
    private IEnumerator InvincibilityFrames()
    {
        isInvicible = true;
        var playerAnimation = PlayerAnimation.Instance;
        var spriteRenderers = new SpriteRenderer[]
        {
            playerAnimation.primaryHandSR,
            playerAnimation.secondaryHandSR
        };

        float invincibilityTime = invincibilityDuration;
        while (invincibilityTime > 0)
        {
            foreach (var renderer in spriteRenderers)
            {
                renderer.enabled = !renderer.enabled;
            }
            yield return new WaitForSeconds(flickerInterval);
            invincibilityTime -= flickerInterval;
        }

        foreach (var renderer in spriteRenderers)
        {
            renderer.enabled = true;
        }

        isInvicible = false;
    }

    public void StartSlowMotionEffect()
    {
        if (inSlowMotion) return;
        StartCoroutine(SlowMotionEffect());
    }

    private IEnumerator SlowMotionEffect()
    {
        inSlowMotion = true;
        float originalScale = Time.timeScale;
        Time.timeScale = slowMotionTimeScale;

        yield return new WaitForSecondsRealtime(slowMotionDuration);

        Time.timeScale = baseTimeScale;
        inSlowMotion = false;
    }

    public void UpdateScore(float change = 1)
    {
        change = change * GetPowerMult(UpgradeType.Rainbow) * GetPowerMult(UpgradeType.Confusion);
        levelScore += change;
        Debug.Log($"Score is now: {levelScore}");

        if(eV != null)
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
        if (fullHealth == 0) return 0f;
        return (float)health / fullHealth;
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
