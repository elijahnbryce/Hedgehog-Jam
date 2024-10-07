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
        SpawnSticker(upgrades[ind].UpgradeType, transform.GetChild(ind).GetChild(0).position);

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

    List<Vector2> GeneratePoints(int count, float startX, float endX, float startY, float endY)
    {
        List<Vector2> pointList = new List<Vector2>();

        float stepX = (endX - startX) / (count - 1);
        float stepY = (endY - startY) / (count - 1);

        for (int i = 0; i < count; i++)
        {
            float x = startX + (i * stepX); // Evenly spaced x values
            float y = startY + (i * stepY); // Evenly spaced y values
            pointList.Add(new Vector2(x, y));
        }

        return pointList;
    }

    List<Vector2> GeneratePoissonPoints(Vector2 bounds, float minDist, int maxAttempts)
    {
        List<Vector2> pointList = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();

        //Vector2 startPoint = new Vector2(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y));
        Vector2 startPoint = Vector2.zero;
        spawnPoints.Add(startPoint);
        pointList.Add(startPoint);

        while (spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool pointAdded = false;

            for (int i = 0; i < maxAttempts; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2);
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 newPoint = spawnCenter + direction * Random.Range(minDist, minDist * 2);

                if (IsInBounds(newPoint, bounds) && IsFarEnough(newPoint, pointList, minDist))
                {
                    pointList.Add(newPoint);
                    spawnPoints.Add(newPoint);
                    pointAdded = true;
                    break;
                }
            }

            if (!pointAdded)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return pointList;
    }

    bool IsInBounds(Vector2 point, Vector2 bounds)
    {
        return point.x >= -bounds.x && point.x <= bounds.x && point.y >= -bounds.y && point.y <= bounds.y;
    }

    bool IsFarEnough(Vector2 point, List<Vector2> points, float minDist)
    {
        foreach (Vector2 p in points)
        {
            if (Vector2.Distance(p, point) < minDist)
                return false;
        }
        return true;
    }
}

[System.Serializable]
public struct UpgradeStruct
{
    public UpgradeType UpgradeType; //rename later lol sorry
    public Sprite UpgradeSprite;

}

public enum UpgradeType
{
    Health = 0,
    MoveSpeed = 1,
    Strength = 2,
}
