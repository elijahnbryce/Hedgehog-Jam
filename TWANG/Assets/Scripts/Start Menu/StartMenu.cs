using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    private void Update()
    {
        // Swap out with UI keyboard controls when there's more than 1 element
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    public void QuitApp()
    {
        Debug.Log("Closing App");
#if UNITY_WEBGL || UNITY_EDITOR
		//note: this function seems unused, just putting this here as a backup
		SceneManager.LoadScene(0);
#else
		Application.Quit();
#endif
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
