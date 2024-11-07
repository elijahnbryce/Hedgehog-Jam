using UnityEngine;

public class Enemy : Entity
{
    public EnemyData data;
    protected bool isDead = false;

    public override void Initialize()
    {
        base.Initialize();

        if (data != null)
        {
            stats.health = data.maxHealth;
            stats.movementSpeed = data.moveSpeed;
            stats.attackPower = data.attackPower;
        }
    }

    // Factory method for creating different enemy types
    public static Enemy CreateEnemy(EnemyType type, Vector3 position)
    {
        GameObject enemyObj = new GameObject($"Enemy_{type}");
        enemyObj.transform.position = position;

        Enemy enemy = enemyObj.AddComponent<Enemy>();
        enemy.data = EnemyDatabase.GetEnemyData(type);

        // Prevent rotation
        enemy.GetComponent<Rigidbody2D>().freezeRotation = true;

        return enemy;
    }
}
