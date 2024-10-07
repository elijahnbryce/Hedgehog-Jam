using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    protected Entity selfEntity;
    //Contains numerical information about the entity

    public float health = 3f;
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
        //if (health <= 0)
        //{
        //    Die();
        //}

        effectiveMovementSpeed = movementSpeed * movementSpeedMult;
    }

    //component-specific methods

    public virtual void TakeDamage(int damage)
    {
        Debug.Log("Enemy Take Damage: " + damage);
        GameManager gm = GameManager.Instance;
        float damageBoost = gm.GetPowerMult(UpgradeType.Fire);
        health -= damage * damageBoost;
        if (health <= 0) {
            Die();
        }
    }

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
        GameManager gm = GameManager.Instance;
        float scoreBoost = gm.GetPowerMult(UpgradeType.Rainbow);
        //play death animation
        //destroy
        gm.RemoveEnemy(selfEntity.gameObject);
        gm.UpdateScore(Mathf.RoundToInt(10 * scoreBoost));
        //Destroy(selfEntity.gameObject);
    }
}
