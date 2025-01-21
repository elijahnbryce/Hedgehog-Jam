using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] TextMeshProUGUI _objectiveText;

    GameObjective[] tutorialObjectives = new GameObjective[]
    {
        new MoveObjective(),
        new AimObjective(),
        new ShootObjective(3),
        new PickupObjective(3)
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
        if (currentObjective >= 0 && currentObjective <= tutorialObjectives.Length)
            Debug.Log($"Completed objective '{tutorialObjectives[currentObjective].Name}'");

        currentObjective++;
        if (currentObjective >= tutorialObjectives.Length)
        {
            // Completed all objectives
            AllObjectivesComplete();
            return;
        }

        Debug.Log($"Starting objective '{tutorialObjectives[currentObjective].Name}'");
        UpdateText();
        tutorialObjectives[currentObjective].OnProgressUpdate += ProgressUpdate;
        tutorialObjectives[currentObjective].OnComplete += NextObjective;
        GameObjectiveManager.Instance.AddObjective(tutorialObjectives[currentObjective]);
    }

    void ProgressUpdate(int progress)
    {
        UpdateText();
    }

    void UpdateText()
    {
        if (currentObjective >= tutorialObjectives.Length) return;
        GameObjective obj = tutorialObjectives[currentObjective];
        _objectiveText.text = $"{obj.Name}\n{obj.Description}: {obj.Progress}/{obj.ProgressCompletion}";
    }

    void AllObjectivesComplete()
    {
        Debug.Log("Objectives complete");
        _objectiveText.text = "";
    }
}
