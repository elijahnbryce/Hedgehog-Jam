using System.Collections;
using System.Collections.Generic;
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

    public List<GameObject> corpses = new();
    public ParticleSystem particles;

    public GameObject followingObject;
    public List<Entity> connectedEntities;

    public virtual void Initialize(Entity thisEntity)
    {
        selfEntity = thisEntity;
    }

    //monobehaviour

    private void Update()
    {
        effectiveMovementSpeed = movementSpeed * movementSpeedMult;
    }

    public virtual void TakeDamage(int damage)
    {
        Debug.Log("Enemy Take Damage: " + damage);
        GameManager gm = GameManager.Instance;
        float damageBoost = gm.GetPowerMult(UpgradeType.Fire);
        health -= damage * damageBoost;
        if (health <= 0)
        {
            Die();

            foreach (var connected in connectedEntities)
            {
                connected.stats.Die(false);
            }
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
        Die(false);
    }

    public virtual void Die(bool ignore)
    {
        bool coins = true;

        GameManager gm = GameManager.Instance;
        if (!ignore)
        {
            gm.StartSlowMotionEffect();
        }

        if (corpses.Count > 0)
        {
            List<GameObject> newCorpses = new();

            if (corpses.Count == 1)
            {
                var newCorpse = Instantiate(corpses[0], transform.position, Quaternion.identity);
                newCorpses.Add(newCorpse);
            }
            else
            {
                Debug.LogError("Issue spawning corpses.");
            }

            gm.RemoveEnemy(selfEntity.gameObject);
        }
        else
        {
            gm.RemoveEnemy(selfEntity.gameObject);
        }

        if (coins && !ignore)
            CoinManager.Instance.SpawnCoins(transform.position);
        SoundManager.Instance.PlaySoundEffect("enemy_die");

        if (particles)
        {
            var particle = Instantiate(particles, transform.position, Quaternion.identity).gameObject;
            Destroy(particle, 2f);
        }
    }
}
