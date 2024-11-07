using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;
using System;

public class EventHandler : MonoBehaviour
{
    public static Action<bool> onPauseUpdate;

    [Header("Canvas")]
    [SerializeField] private GameObject pauseMen;
    [SerializeField] private GameObject loseCan, ovrCan;

    [SerializeField] private TextMeshProUGUI endScore, endTime, endKills, goalText, healthText, hsText, timerText, gradeText;
    [SerializeField] private TMP_InputField hsInput;

    [Header("Objects")]
    private Camera cam;
    //[SerializeField] public GameObject timerObject;
    private GameManager gm;

    public Timer ts;

	private void Awake()
	{
		gm = GetComponent<GameManager>();
	}

	private void Start()
    {        
        ts = GetComponent<Timer>();
		UpdateCursor();
    }

	private void OnApplicationFocus(bool focus)
	{
		UpdateCursor();
	}

    void UpdateCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UIFalse()
    {
        //nxtCan.SetActive(false);
        ovrCan.SetActive(false);
        //hsInput.gameObject.SetActive(false);
        //winCan.SetActive(false);
        loseCan.SetActive(false);
        OnPauseChanged(false);
    }

    public void DisplayHealth(int hp)
    {
        healthText.text = "Lives: " + hp.ToString();
    }

    public void DisplayScore(float lvlScore)
    {
        goalText.text = "Score: " + lvlScore.ToString("C");
    }

    public void CheckHS(int totalScore)
    {
        //cam.GetComponent<AudioSource>().Stop();
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

    private string GetGrade(float score, int guide)
    {
        Debug.Log(guide);
        
        float grade = (float)score / guide;
        string letter;
        if (grade > 1) letter = "A";
        else if (grade >= .9f) letter = "A";
        else if (grade >= .8f) letter = "B";
        else if (grade >= .7f) letter = "C";
        else if (grade >= .6f) letter = "D";
        else letter = "F";
        
        Debug.Log(grade.ToString());
        return letter;
    }

    public void LoseGame(float finalScore, int guideScore)
    {
        Debug.Log($"Lost Game. Score {finalScore}");
        gm.gameOver = true;
        ovrCan.SetActive(true);
        loseCan.SetActive(true);
        endScore.text = finalScore.ToString("C");
        ts.DisplayTime(endTime);
        endKills.text = gm.enemiesKilled.ToString();
        gradeText.text = GetGrade(finalScore, guideScore);

        //CheckHS(finalScore);
    }

    public void LoadScene(int sceneNum = 1)
    {
        SceneManager.LoadScene(sceneNum);
    }

	//should be moved/renamed but don't want to break the scene references
    public void PauseGame()
    {
        Debug.Log("Paused");
		gm.PauseGame();
	}

	public void OnPauseChanged(bool p)
	{
		pauseMen.SetActive(p);
        onPauseUpdate?.Invoke(p);

        UpdateCursor();
	}

    public void Restart()
    {
        Time.timeScale = 1;
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

	public void ReturnToMenu()
	{
		SceneManager.LoadScene(0);
	}

    public void QuitApp()
    {
#if UNITY_WEBGL || UNITY_EDITOR
        if (SceneManager.GetActiveScene().buildIndex != 0)
            SceneManager.LoadScene(0);
#else
		Application.Quit();
#endif
    }
}
