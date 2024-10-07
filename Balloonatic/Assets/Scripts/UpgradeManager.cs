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
        //for (int i = 0; i < 4; i++)
        //{
        //    SpawnSticker((UpgradeType)Random.Range(0, 3));
        //    //SpawnSticker(UpgradeType.Health);
        //}
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SpawnUpgrades();
        //}
    }

    public void SpawnUpgrades()
    {
        HelperClass.Shuffle(upgrades);

        var ind = 0;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            //child.Translate(Vector2.up * 10);
            child.transform.position = new Vector2(child.transform.position.x, 10);

            var newSeq = DOTween.Sequence();
            newSeq.AppendInterval(ind * 0.1f);
            newSeq.Append(child.DOMove(child.position - (Vector3.up * 10), 0.25f));
            child.DOShakeRotation(0.25f);
            //this looks so awesome, thank me later :)

            var strct = upgrades[ind];
            child.GetChild(0).GetComponent<SpriteRenderer>().GetComponent<SpriteRenderer>().sprite = strct.UpgradeSprite;

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
        GameManager gm = GameManager.Instance;
        UpgradeType upgrade = upgrades[ind].UpgradeType;
        gm.AddPowerUP(upgrade);

        SpawnSticker(upgrade, transform.GetChild(ind).GetChild(0).position);

        for (int i = 0; i < 2; i++)
        {
            transform.GetChild(i).GetChild(0).gameObject.SetActive(ind!=i);
        }

        UnspawnUpgrades();
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

        //var newSticker = Instantiate(stickerPrefab, spawnPos, Quaternion.identity);
        //newSticker.transform.Rotate(0, 0, Random.Range(-60, 60));

        newSticker.transform.DOLocalRotate(new Vector3(0, 0, Random.Range(-60, 60)), 1f);
        newSticker.transform.DOMove(stickerFinalPos, 1f);
    }
}

[System.Serializable]
public struct UpgradeStruct
{
    public UpgradeType UpgradeType; //rename later lol sorry
    public Sprite UpgradeSprite;
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
    Shoe = 1,
    Pizza = 2,
    Rainbow = 3,
    Star = 4,
    Lightning = 5,
    Fire = 6,
    Knife = 7,
}
