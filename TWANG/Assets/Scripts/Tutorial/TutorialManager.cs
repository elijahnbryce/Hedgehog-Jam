using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    GameObjective[] tutorialObjectives = new GameObjective[]
    {
        new MoveObjective(),
        new AimObjective()
    };
    int currentObjective = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    private void Start()
    {
        NextObjective();
    }

    void NextObjective()
    {
        if(currentObjective >= 0 && currentObjective <= tutorialObjectives.Length)
            Debug.Log($"Completed objective '{tutorialObjectives[currentObjective].Name}'");
        
        currentObjective++;
        if (currentObjective >= tutorialObjectives.Length)
        {
            // Completed all objectives
            return;
        }

        Debug.Log($"Starting objective '{tutorialObjectives[currentObjective].Name}'");
        tutorialObjectives[currentObjective].OnComplete += NextObjective;
        GameObjectiveManager.Instance.AddObjective(tutorialObjectives[currentObjective]);
    }
}
