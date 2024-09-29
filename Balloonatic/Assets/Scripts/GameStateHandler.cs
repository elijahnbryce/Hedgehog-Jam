using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateHandler : MonoBehaviour
{
    public static GameStateHandler _Instance;

    public int cLVL = 1, fLVL = 3, pHP = 3, score = 0, lastLVL;
    public bool launch;

    private void Awake()
    {
        _Instance = this;
        DontDestroyOnLoad(gameObject);
        lastLVL = SceneManager.sceneCountInBuildSettings - 1;
        cLVL = SceneManager.GetActiveScene().buildIndex;
        launch = true;
    }

    public int getLevel()
    {
        return cLVL;
    }

    public int getHealth()
    {
        return pHP;
    }

    public int getScore()
    {
        return score;
    }

    public void incLevel()
    {
        cLVL++;
        LoadNext();
        //Invoke("LoadNext", 1f);
    }

    public void incHealth(int val = 1)
    {
        pHP += val;
    }

    public void incScore(int val)
    {
        score += val;
    }

    public void setLevel(int val)
    {
        cLVL = val;
    }

    public void setHealth(int val)
    {
        pHP = val;
    }

    public void setScore(int val)
    {
        score = val;
    }

    public bool isFinal()
    {
        return (cLVL == lastLVL);
    }

    public void LoadNext()
    {
        launch = false;
        SceneManager.LoadScene(cLVL);
    }

    public void restartGame(EventHandler eV)
    {
        KillSelf();
        eV.LoadScene();
    }

    public void KillSelf()
    {
        _Instance = null;
        Destroy(gameObject);
    }
}
