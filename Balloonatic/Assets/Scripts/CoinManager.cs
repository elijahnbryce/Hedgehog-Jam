using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private List<CoinStruct> coins = new();
    [SerializeField] private Coin coinPrefab;

    public static CoinManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void SpawnCoins(Vector2 pos)
    {
        for (int i = 0; i < Random.Range(1,5); i++)
        {
            SpawnCoin(GetRandomCoinType(), pos + new Vector2(Random.Range(-1f,1) * .5f, Random.Range(-1f,1) * .5f));
        }
    }

    public void SpawnCoin(CoinType coinType, Vector2 pos)
    {
        var newCoin = Instantiate(coinPrefab, pos, Quaternion.identity);
        newCoin.InitializeCoin(coins.Where(c => c.Type.Equals(coinType)).ToList()[0]);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private CoinType GetRandomCoinType()
    {
        float randomValue = Random.value;
        if (randomValue < 0.75f) return CoinType.Penny;
        if (randomValue < 0.90f) return CoinType.Nickel;
        if (randomValue < 0.97f) return CoinType.Dime;
        return CoinType.Quarter;
    }
}

[System.Serializable]
public struct CoinStruct
{
    public CoinType Type;
    public List<Sprite> Sprites;
}

public enum CoinType
{
    Penny = 1,
    Nickel = 5,
    Dime = 10,
    Quarter = 25
}
