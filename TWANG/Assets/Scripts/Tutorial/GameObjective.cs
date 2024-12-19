using System;
using UnityEngine;

public abstract class GameObjective
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public int Progress
    {
        get => _progress;
        protected set
        {
            if (_progress != value)
            {
                _progress = value;
                OnProgressUpdate?.Invoke(_progress);
            }
        }
    }
    private int _progress;

    public int ProgressCompletion { get; protected set; } = 1;

    public Action OnComplete;
    public Action<int> OnProgressUpdate;

    public bool IsComplete { get; private set; }

    public virtual void StartObjective() { }
    public virtual void UpdateObjective() { }

    protected void CompletedObjective()
    {
        // Only complete once
        if (IsComplete) return;

        IsComplete = true;
        Progress = ProgressCompletion;
        OnComplete?.Invoke();
    }
}