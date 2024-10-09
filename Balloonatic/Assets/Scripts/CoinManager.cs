using System.Collections;
using System.Collections.Generic;
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
