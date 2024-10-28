using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EventHandler : MonoBehaviour
{

    [Header("Canvas")]
    [SerializeField] private GameObject pauseMen;
    [SerializeField] private GameObject loseCan, ovrCan;

    [SerializeField] private TextMeshProUGUI endScore, endTime, endKills, goalText, healthText, hsText, timerText, gradeText;
    [SerializeField] private TMP_InputField hsInput;


    [Header("Objects")]
    private Camera cam;
    //[SerializeField] public GameObject timerObject;

    private static GameManager gm = GameManager.Instance;
    public Timer ts;

    private void Start()
    {
        gm = GameManager.Instance;
        ts = GetComponent<Timer>();
		UpdateCursor();
    }

	private void OnApplicationFocus(bool focus)
	{
		UpdateCursor();
	}

	public void UIFalse()
    {
        //nxtCan.SetActive(false);
        ovrCan.SetActive(false);
        //hsInput.gameObject.SetActive(false);
        //winCan.SetActive(false);
        loseCan.SetActive(false);
        pauseMen.SetActive(false);
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

    private string GetGrade(int score, int guide)
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

    public void LoseGame(int finalScore, int guideScore)
    {
        Debug.Log("Lost Game");
        gm.gameOver = true;
        ovrCan.SetActive(true);
        loseCan.SetActive(true);
        endScore.text = finalScore.ToString();
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
		gm.PauseGame();
	}

	public void OnPauseChanged(bool p)
	{
		pauseMen.SetActive(p);
		UpdateCursor();
	}

    public void Restart()
    {
        Time.timeScale = 1;
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitApp()
    {
#if UNITY_WEBGL
		SceneManager.LoadScene(0);
#else
		Application.Quit();
#endif
    }

	public void UpdateCursor()
	{
		if (gm.gamePaused)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		else
		{
			//Cursor.lockState = CursorLockMode.None;  //tbd
			Cursor.visible = false;
		}
	}
}
