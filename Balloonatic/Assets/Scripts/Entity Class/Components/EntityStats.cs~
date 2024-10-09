using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    protected Entity selfEntity;
    //Contains numerical information about the entity

    public float health = 100f;
    public float movementSpeed = 10f;
    public float movementSpeedMult = 1f;
    public float effectiveMovementSpeed = 0;

    public float attackPower = 1f;
    public float attackPowerMult = 1f;

    public virtual void Initialize(Entity thisEntity)
    {
        selfEntity = thisEntity;
    }

    //monobehaviour

    private void Update()
    {
        if (health <= 0)
        {
            Die();
        }

        effectiveMovementSpeed = movementSpeed * movementSpeedMult;
    }

    //component-specific methods

    public virtual void IncrementHealth(float increment)
    {
        health += increment;
    }

    public virtual void SetHealth(float amount)
    {
        health = amount;
    }

    public virtual void Die()
    {
        //play death animation
        //destroy
        Destroy(selfEntity.gameObject);
    }
}
