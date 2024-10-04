using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hsText;
    //private FlyInBoardBlank flyIn;

    private void Start()
    {
        if (PlayerPrefs.GetString("HIGHSCORENAME") != "")
        {
            hsText.text = "HIGHSCORE: " + PlayerPrefs.GetString("HIGHSCORENAME") + " -> " + PlayerPrefs.GetInt("HIGHSCORE");
        }
        //flyIn = GetComponent<FlyInBoardBlank>();
        //flyIn.SequenceFlyIn(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void QuitApp()
    {
        Debug.Log("Closing App");
        Application.Quit();
    }
}
