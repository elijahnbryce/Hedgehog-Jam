using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    public EnemyType enemyType;
    public float maxHealth;
    public float moveSpeed;
    public float attackPower;
    public float attackRange;
    public float attackCooldown;
}