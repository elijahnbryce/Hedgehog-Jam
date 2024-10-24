using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public void QuitWithEffect(Button butt)
    {
        GetComponent<PencilScribble>().SquiggleSquaggle(butt, QuitApp);
    }

    public void StartWithEffect(Button butt)
    {
        GetComponent<PencilScribble>().SquiggleSquaggle(butt, StartGame);
    }
}
