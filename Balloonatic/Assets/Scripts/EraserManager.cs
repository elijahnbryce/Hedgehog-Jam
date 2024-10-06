using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EraserManager : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites = new();
    [SerializeField] private List<Sprite> colliderSprites = new();
    [SerializeField] private List<Sprite> crackedSprites = new();
    [SerializeField] private List<GameObject> configs = new();
    [SerializeField] private Material whiteMat, defaultMat;
    public static EraserManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    void Start()
    {
        SpawnConfig();
    }

    public Sprite GetColliderSprite(int index)
    {
        return colliderSprites[index];
    }

    public Sprite GetCrackedSprite(int index)
    {
        return crackedSprites[index];
    }

    void Update()
    {

    }

    public void SpawnConfig()
    {
        var chosenConfig = Instantiate(configs[Random.Range(0, configs.Count)]).transform;
        chosenConfig.parent = transform;
        chosenConfig.transform.Translate(Vector3.up * 20f);

        //Destroy(chosenConfig.gameObject);

        var bools = GenerateBools(chosenConfig.childCount);

        var ind = 0;
        foreach (Transform child in chosenConfig)
        {
            child.gameObject.SetActive(bools[ind]);
            var seq = DOTween.Sequence();
            seq.AppendInterval(ind++ * 0.1f);
            seq.Append(child.DOMove(child.position - Vector3.up * 20f, 1f));
            seq.AppendCallback(() =>
            {
                foreach (Transform child2 in child)
                {
                    child2.GetComponent<SpriteRenderer>().material = whiteMat;
                }
                child.DOPunchScale(Vector2.one * 0.1f, 0.1f);
            });
            seq.AppendInterval(0.1f);
            seq.AppendCallback(() =>
            {
                foreach (Transform child2 in child)
                {
                    child2.GetComponent<SpriteRenderer>().material = defaultMat;
                }
            });
        }
    }

    private List<bool> GenerateBools(int amount)
    {
        var list = new List<bool>();
        for (int i = 0; i < amount / 2; i++)
        {
            list.Add(true);
        }
        while (list.Count < amount)
        {
            list.Add(Random.value > 0.5f);
        }
        HelperClass.Shuffle(list);
        return list;
    }

    public void UnspawnConfig()
    {

    }
}
