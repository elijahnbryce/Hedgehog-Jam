using System.Collections.Generic;

public static class EnemyDatabase
{
    private static Dictionary<EnemyType, EnemyData> enemyData;

    static EnemyDatabase()
    {
        // Load all enemy data scriptable objects
        // This would be populated from your ScriptableObjects
        enemyData = new Dictionary<EnemyType, EnemyData>();
    }

    public static EnemyData GetEnemyData(EnemyType type)
    {
        return enemyData.TryGetValue(type, out EnemyData data) ? data : null;
    }
}