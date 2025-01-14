using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private List<UpgradeStruct> upgrades = new();
    [SerializeField] private GameObject stickerPrefab;

    [SerializeField] private Vector2 bounds;
    private int maxAttempts = 30;

    private List<Vector2> points = new List<Vector2>();
    public static UpgradeManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    //void Update()
    //{

    //}

    public void SpawnUpgrades()
    {
        EraserManager.Instance.UnspawnConfig();

        HelperClass.Shuffle(upgrades);

        var ind = 0;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            child.transform.position = new Vector2(child.transform.position.x, 10);

            var newSeq = DOTween.Sequence();
            newSeq.AppendInterval(ind * 0.1f);
            newSeq.Append(child.DOMove(child.position - (Vector3.up * 10), 0.25f));
            newSeq.AppendCallback(() => SoundManager.Instance.PlaySoundEffect("upgrades_spawn"));
            child.DOShakeRotation(0.25f);
            var strct = upgrades[ind];
            child.GetChild(0).GetComponent<SpriteRenderer>().sprite = strct.UpgradeSprite;
            child.GetComponent<SpriteRenderer>().sprite = strct.PaperSprite;

            ind++;
        }
    }

    private void UnspawnUpgrades()
    {
        var ind = 0;
        foreach (Transform child in transform)
        {
            var newSeq = DOTween.Sequence();

            newSeq.AppendInterval(ind * 0.1f);
            newSeq.Append(child.DOMove(child.position - (Vector3.up * 10), 0.25f));
            child.DOShakeRotation(0.25f).OnComplete(() =>
            {
                child.gameObject.SetActive(false);
            });

            ind++;
        }
    }

    public void ClaimUpgrade(int ind)
    {
        SoundManager.Instance.PlaySoundEffect("upgrade_claim");
        GameManager gm = GameManager.Instance;
        UpgradeType upgrade = upgrades[ind].UpgradeType;

        while (upgrade == UpgradeType.Question)
        {
            upgrade = upgrades[Random.Range(0, upgrades.Count)].UpgradeType;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.childCount > 0)  
            {
                child.GetChild(0).gameObject.SetActive(ind != i);
            }
        }

        switch (upgrade)
        {
            //repeated code, fix later
            case UpgradeType.Penny:
                CoinManager.Instance.SpawnCoin(CoinType.Penny, PlayerMovement.Instance.PlayerPosition - Vector2.up * 2);
                break;
            case UpgradeType.Nickel:
                CoinManager.Instance.SpawnCoin(CoinType.Nickel, PlayerMovement.Instance.PlayerPosition - Vector2.up * 2);
                break;
            case UpgradeType.Dime:
                CoinManager.Instance.SpawnCoin(CoinType.Dime, PlayerMovement.Instance.PlayerPosition - Vector2.up * 2);
                break;
            case UpgradeType.Quarter:
                CoinManager.Instance.SpawnCoin(CoinType.Quarter, PlayerMovement.Instance.PlayerPosition - Vector2.up * 2);
                break;
            default:
                gm.AddPowerUP(upgrade);
                SpawnSticker(upgrade, transform.GetChild(ind).GetChild(0).position);
                break;
        }

        UnspawnUpgrades();
        GameManager.Instance.NewWave();
    }

    private void SpawnSticker(UpgradeType type, Vector2 spawnPos)
    {
        var newSticker = Instantiate(stickerPrefab, spawnPos, Quaternion.identity);
        newSticker.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = upgrades.Where(u => u.UpgradeType.Equals(type)).ToList()[0].UpgradeSprite;

        var stickerFinalPos = Vector2.zero;
        var tries = 0;
        var keepGoing = true;
        while (tries++ < maxAttempts && keepGoing)
        {
            stickerFinalPos = new Vector2(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y));

            keepGoing = false;
            foreach (var point in points)
            {
                if (Vector2.Distance(stickerFinalPos, point) < 2)
                {
                    keepGoing = true;
                }
            }
        }
        points.Add(stickerFinalPos);

        newSticker.transform.position = stickerFinalPos;
        newSticker.GetComponent<Sticker>().Set();
        SoundManager.Instance.PlaySoundEffect("sticker_apply");
    }
}

[System.Serializable]
public struct UpgradeStruct
{
    public UpgradeType UpgradeType;
    public Sprite UpgradeSprite;
    public Sprite PaperSprite;
    public int AmountClaimed;
}

public enum UpgradeType
{
    Heart = 0,
    Pizza = 1,
    Rainbow = 2, 
    Star = 3, //should work
    Lightning = 4, //
    Fire = 5,
    Ghost = 6,
    Question = 7,
    Confusion = 8,
    Evil_Pizza = 9,
    Penny = 10,
    Nickel = 11,
    Dime = 12,
    Quarter = 13
}
