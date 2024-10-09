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
    private float minimumDistance = 1f;
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

    void Update()
    {

    }

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
            ind = Random.Range(0, 13+1);
            upgrade = upgrades[ind].UpgradeType;
        }

        for (int i = 0; i < 2; i++)
        {
            transform.GetChild(i).GetChild(0).gameObject.SetActive(ind != i);
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

        //new wave shouldnt be called here, because of coin claiming
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

/*
 * heart - full health: resets health to 4 when claimed
shoe - speed increase: 10% speed increase capped at 2x speed
pizza - heal (??) Perhaps on the first damage taken heal 1 health
rainbow - point multiplier
star - invincible for 10s: the first 10s of the round player will be unable to take damage
lightning - elijah's lightning attack: lightning splash damage 10% chance for applying
fire - damage bonus: 10% chance to deal double damage with a fire band
knife - piercing: when a band kills, it keeps going x times where x is the amount of times this upgrade was claimed
 */

public enum UpgradeType
{
    Heart = 0,
    Pizza = 1,
    Rainbow = 2,
    Star = 3,
    Lightning = 4,
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
