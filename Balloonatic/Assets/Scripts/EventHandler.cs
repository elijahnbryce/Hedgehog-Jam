using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EventHandler : MonoBehaviour
{

    [Header("Canvas")]
    [SerializeField] private GameObject pauseMen;
    [SerializeField] private GameObject winCan, loseCan, ovrCan, nxtCan;

    [SerializeField] private TextMeshProUGUI loseScore, winScore, goalText, healthText, hsText, timerText;
    [SerializeField] private TMP_InputField hsInput;


    [Header("Objects")]
    private Camera cam;
    //[SerializeField] public GameObject timerObject;

    [Header("Game Status")] 
    public int health = 5;
    public bool gameOver, gameActive, gamePaused;
    private int levelScore, totalScore = 0;

    //private GameManager gm = GameManager.Instance;
    //private Timer ts;


    private void Start()
    {
        SetLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }

    private void SetLevel()
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
        nxtCan.SetActive(false);
        ovrCan.SetActive(false);
        hsInput.gameObject.SetActive(false);
        winCan.SetActive(false);
        loseCan.SetActive(false);
        pauseMen.SetActive(false);
        //ts.StartTime();

        gameOver = false;
        gameActive = true;
        gamePaused = false;
    }

    private void NewWave()
    {
        Debug.Log("New wave");
        // incriment level count
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
        //  or multiply by (1 - enemies.Count / enemies2Spawn);
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
            LoseGame(totalScore);
        }
    }

    private void CheckHS()
    {
        cam.GetComponent<AudioSource>().Stop();
        ovrCan.SetActive(true);
        int highScore = PlayerPrefs.GetInt("HIGHSCORE");
        if (totalScore > highScore)
        {
            PlayerPrefs.SetInt("HIGHSCORE", totalScore);
            hsText.text = "New High Score!";
            hsInput.gameObject.SetActive(true);
        }
        else
        {
            hsText.text = "*+ " + PlayerPrefs.GetString("HIGHSCORENAME") + ": " + highScore + " +*";
        }
        // lvlText.text = gm.getGLVL().ToString();
    }

    public void NewHighScore()
    {
        string hsName = hsInput.text;
        PlayerPrefs.SetString("HIGHSCORENAME", hsName);
        hsInput.gameObject.SetActive(false);
    }

    private void LoseGame(int finalScore)
    {
        gameOver = true;
        loseCan.SetActive(true);
        loseScore.text = finalScore.ToString();

        CheckHS();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //private void WinGame(int finalScore)
    //{
    //    winCan.SetActive(true);
    //    winScore.text = finalScore.ToString();
    //}

    public void LoadScene(int sceneNum = 1)
    {
        SceneManager.LoadScene(sceneNum);
    }

    public void PauseGame()
    {
        if (gameOver) return;
        if (gamePaused)
        {
            pauseMen.SetActive(false);
            gamePaused = false;
            gameActive = true;
            Time.timeScale = 1;

            //Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Time.timeScale = 0;
            gameActive = false;
            gamePaused = true;
            pauseMen.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Restart()
    {
        if (gamePaused)
        {
            LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else LoadScene();
    }

    public void QuitApp()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Application.Quit();
        }
        else LoadScene(0);
    }

    public bool getGameActive()
    {
        return (gameActive && !gamePaused);
    }

    public void UpdateHealth(int change = -1)
    {
        health += change;
        healthText.text = "Lives: " + health.ToString();

        if (health <= 0)
        {
            health = 0;
            EndLevel(false, levelScore);
        }
    }

    public void UpdateScore(int change = 1)
    {
        levelScore += change;
        goalText.text = "Score: " + levelScore.ToString();
    }
}