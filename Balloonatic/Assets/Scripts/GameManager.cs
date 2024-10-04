using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.WSA;

public class GameManager : MonoBehaviour
{
    public int cLVL = 1, fLVL = 3, pHP = 3, score = 0, lastLVL;
    public bool launch;

    public static GameManager Instance { get; private set; }
    private EventHandler eV;

    [Header("Game Status")]
    [SerializeField] private static int fullHealth = 3;
    public int health = fullHealth, wave = 0;
    public bool gameOver, gameActive, gamePaused;
    private int levelScore, totalScore = 0;
    
    //private Timer ts;

    private Camera cam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Game Manager Already Exists");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Game Manager Set");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        eV = GetComponent<EventHandler>();
        SetLevel();
    }

    public void ReStart()
    {
        SetLevel(true);
    }

    public void Kill()
    {
        Instance = null;
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

        NewWave(restart);
    }

    private void EverythingFalse()
    {
        eV.UIFalse();

        gameOver = false;
        gameActive = true;
        gamePaused = false;
    }

    private void NewWave(bool restart = false)
    {
        if (restart) return;
        Debug.Log("New wave");
        wave++;
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
        Time.timeScale = 0;
        gameActive = false;

        totalScore += CalcLevelScore(score);

        if (alive)
        {
            // wave intermission
            NewWave();
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

    //public void RemoveEnemy(GameObject other)
    //{
    //    levEnemies.Remove(other);
    //    Destroy(other);
    //}

    //private int GetFinalScore(int score, float time = 10f)
    //{
    //    return Mathf.RoundToInt((1 + (float)score / (float)(fullHealth / 2)) * (1 - (1 - time / 10f)) * (1 + (float)(startEnemies - levEnemies.Count) / startEnemies) * ((float)(startPickups - levPickups.Count) / startPickups) * 1017); // score is just health
    //}

    //private void LoadEnemies()
    //{
    //    foreach (Transform child in enemyHolder)
    //    {
    //        levEnemies.Add(child.gameObject);
    //    }

    //    remEnemies = levEnemies.Count;
    //    startEnemies = (remEnemies == 0) ? 1 : remEnemies;
    //}

    public void UpdateHealth(int change = -1)
    {
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
        eV.DisplayHealth(levelScore);
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
}
