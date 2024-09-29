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
    public bool gameOver, gameActive, gamePaused, noPausing;
    private int levelScore, totalScore = 0;

    //private GameManager gm = GameManager._Instance;
    private static GameStateHandler gs = GameStateHandler._Instance;
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

        //highestScore = Mathf.RoundToInt((1 + phFull / (float)(phFull / 2)) * (1 - (1 - 10f / 10f)) * (1 + (startEnemies - 0) / startEnemies) * ((startPickups - 0) / startPickups) * 1017);
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
        noPausing = false;
    }

private void IncLevel()
    {
        Debug.Log("ICREASING LEVEL");
        //currLVL++;
        SetLevel();
    }

    public void Restart()
    {
        if (gamePaused)
        {
            if (gs.launch) gs.KillSelf();
            LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else gs.restartGame(this); // restart current level for testing
    }

    public void EndLevel(bool result, int score)
    {
        Time.timeScale = 0;
        gameActive = false;
        totalScore += score;

        //float timeElapsed = ts.GetTime();
        //float timeMult = (timeElapsed / 60 < 4) ? (1 - timeElapsed / 300) : 0.1f;

        //totalScore = Mathf.RoundToInt(totalScore * (1 + (float)health / 10) * timeMult * 1017);
        gs.incScore(totalScore);

        if (result)
        {
            WinGame(gs.getScore());
            // next level
        }
        else
        {
            LoseGame(gs.getScore());
        }

        CheckHS();
        noPausing = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
        loseCan.SetActive(true);
        loseScore.text = finalScore.ToString();
    }

    private void WinGame(int finalScore)
    {
        winCan.SetActive(true);
        winScore.text = finalScore.ToString();
    }

    public void LoadScene(int sceneNum = 1)
    {
        SceneManager.LoadScene(sceneNum);
    }

    public void PauseGame()
    {
        if (noPausing) return;
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

    public void QuitApp()
    {
        gs?.KillSelf();
        LoadScene(0);
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