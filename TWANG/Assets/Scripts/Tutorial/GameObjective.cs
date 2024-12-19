using System;
using UnityEngine;

public abstract class GameObjective
{
    public string Name;
    public string Description;
    public Action OnComplete;

    public bool IsComplete => _isComplete;
    private bool _isComplete;

    public virtual void StartObjective() { }
    public virtual void UpdateObjective() { }

    protected void CompletedObjective()
    {
        // Only complete once
        if (_isComplete) return;

        _isComplete = true;
        OnComplete?.Invoke();
    }
}