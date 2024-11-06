using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStateMachine : MonoBehaviour
{
    protected Entity selfEntity;

    public List<EntityState> states = new List<EntityState>();
    public int numStatesActivated = 0;

    public virtual void Initialize(Entity thisEntity)
    {
        selfEntity = thisEntity;
        if (selfEntity.entityStates.Count > 0)
        {
            foreach (Entity.EntityStateInitializer initializer in selfEntity.entityStates)
            {
                if(initializer.state == null)
                {
                    Debug.LogWarning($"Attempted to instantiate null state on {selfEntity.name}");
                    continue;
                }
                EntityState newState = Instantiate(initializer.state);
                newState.Initialize(thisEntity, initializer.entityStateChangers);
                states.Add(newState);
                newState.isActive = initializer.active;
            }
        }
    }

    //monobehaviour loops
    private void Update()
    {
        foreach (EntityState state in states)
        {
            if (state.isActive)
            {
                state.Update();
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (EntityState state in states)
        {
            if (state.isActive)
            {
                state.FixedUpdate();
            }
        }
    }

    //component-specific methods
}
