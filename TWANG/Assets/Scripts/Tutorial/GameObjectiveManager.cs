using System.Collections.Generic;
using UnityEngine;

public class GameObjectiveManager : MonoBehaviour
{
    public static GameObjectiveManager Instance;
    private List<GameObjective> _gameObjectives = new List<GameObjective>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void AddObjective(GameObjective newObjective)
    {
        _gameObjectives.Add(newObjective);
        newObjective.StartObjective();
    }

    private void Update()
    {
        int objIndex = 0;
        while (objIndex < _gameObjectives.Count)
        {
            _gameObjectives[objIndex].UpdateObjective();
            if (_gameObjectives[objIndex].IsComplete)
                _gameObjectives.RemoveAt(objIndex);
            else
                objIndex++;
        }
    }
}