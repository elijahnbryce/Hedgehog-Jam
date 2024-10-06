using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EraserManager : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites = new();
    [SerializeField] private List<Sprite> colliderSprites = new();
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

    void Update()
    {
        
    }

    public void SpawnConfig()
    {
        var chosenConfig = Instantiate(configs[Random.Range(0, configs.Count)]).transform;
        chosenConfig.parent = transform;
        chosenConfig.transform.Translate(Vector3.up * 20f);

        //Destroy(chosenConfig.gameObject);

        var ind = 0;
        foreach(Transform child in chosenConfig)
        {
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

    public void UnspawnConfig()
    {

    }
}
