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
            //DontDestroyOnLoad(gameObject);
        }
        eV = GetComponent<EventHandler>();
        ts = GetComponent<Timer>();

        DOTween.SetTweensCapacity(500, 50);
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

    private void EverythingFalse()
    {
        eV.UIFalse();
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

        float fadeTime = 0.25f; // 1.5s total / 6 transitions = 0.25s per fade
        float endAlpha = 0.5f;

        // 3 full cycles
        for (int cycle = 0; cycle < 3; cycle++)
        {
            // Fade to 50% transparent
            float elapsed = 0f;
            float startAlpha = 1f;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeTime);

                foreach (var renderer in spriteRenderers)
                {
                    var color = renderer.color;
                    color.a = alpha;
                    renderer.color = color;
                }

                yield return null;
            }

            // Fade back to fully opaque
            elapsed = 0f;
            startAlpha = endAlpha;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, 1f, elapsed / fadeTime);

                foreach (var renderer in spriteRenderers)
                {
                    var color = renderer.color;
                    color.a = alpha;
                    renderer.color = color;
                }

                yield return null;
            }
        }

        // Ensure sprites are fully visible when done
        foreach (var renderer in spriteRenderers)
        {
            var color = renderer.color;
            color.a = 1f;
            renderer.color = color;
        }

        isInvicible = false;
    }

    public void UpdateScore(float change = 1)
    {
        change = change * GetPowerMult(UpgradeType.Rainbow) * GetPowerMult(UpgradeType.Confusion);
        levelScore += change;
        Debug.Log($"Score is now: {levelScore}");
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
}
