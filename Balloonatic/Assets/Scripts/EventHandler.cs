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

    private GameManager gm = GameManager.Instance;
    //private Timer ts;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }

    public void UIFalse()
    {
        nxtCan.SetActive(false);
        ovrCan.SetActive(false);
        hsInput.gameObject.SetActive(false);
        winCan.SetActive(false);
        loseCan.SetActive(false);
        pauseMen.SetActive(false);
        //ts.StartTime();
    }

    public void DisplayHealth(int hp)
    {
        healthText.text = "Lives: " + hp.ToString();
    }

    public void DisplayScore(int lvlScore)
    {
        goalText.text = "Score: " + lvlScore.ToString();
    }

    public void CheckHS(int totalScore)
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

    public void LoseGame(int finalScore)
    {
        gm.gameOver = true;
        loseCan.SetActive(true);
        loseScore.text = finalScore.ToString();

        CheckHS(finalScore);
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
        if (gm.gameOver) return;
        if (gm.gamePaused)
        {
            pauseMen.SetActive(false);
            gm.gamePaused = false;
            gm.gameActive = true;
            Time.timeScale = 1;

            //Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Time.timeScale = 0;
            gm.gameActive = false;
            gm.gamePaused = true;
            pauseMen.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Restart()
    {
        if (gm.gamePaused)
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
}