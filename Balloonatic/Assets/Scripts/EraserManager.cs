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
    [SerializeField] private ParticleSystem eraserParticles;
    public static EraserManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    void Start()
    {
        //remove later
        //SpawnConfig();
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
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    UnspawnConfig();
        //}
    }

    public void SpawnConfig()
    {
        // Clear existing children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Ensure configs list is not empty
        if (configs.Count == 0)
        {
            Debug.LogError("No configs available in EraserManager.");
            return;
        }

        // Instantiate new config
        GameObject chosenConfigObj = Instantiate(configs[Random.Range(0, configs.Count)], transform);
        Transform chosenConfig = chosenConfigObj.transform;
        chosenConfig.localPosition = Vector3.up * 20f;

        // Generate bools for child activation
        List<bool> bools = GenerateBools(chosenConfig.childCount);

        // Process each child
        for (int ind = 0; ind < chosenConfig.childCount; ind++)
        {
            Transform child = chosenConfig.GetChild(ind);
            child.gameObject.SetActive(bools[ind]);

            // Create a local copy of ind for the closure
            int capturedInd = ind;

            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(capturedInd * 0.1f);
            seq.Append(child.DOMove(child.position - Vector3.up * 20f, 1f));
            seq.AppendCallback(() =>
            {
                foreach (Transform child2 in child)
                {
                    SpriteRenderer spriteRenderer = child2.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.material = whiteMat;
                    }
                }
                child.DOPunchScale(Vector2.one * 0.1f, 0.1f);
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySoundEffect("eraser_spawn");
                }
            });
            seq.AppendInterval(0.1f);
            seq.AppendCallback(() =>
            {
                foreach (Transform child2 in child)
                {
                    SpriteRenderer spriteRenderer = child2.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.material = defaultMat;
                    }
                }

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.BetweenRounds = false;
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
            list.Add(UnityEngine.Random.value > 0.5f);
        }
        HelperClass.Shuffle(list);
        return list;
    }

    public void UnspawnConfig()
    {
        GameManager.Instance.BetweenRounds = true;

        //Destroy(transform.GetChild(0).gameObject);
        var ind = 0;
        foreach (Transform child in transform)
        {
            ind++;
            //child.DORotate(new Vector3(0, 0, 180), ind * 0.1f + 1f);
            //var seq = DOTween.Sequence();
            //seq.AppendInterval(ind * 0.1f);
            //seq.Append(child.DOMove(child.position - Vector3.up * 20f, 1f));

            foreach (Transform child2 in child)
            {
                var sr = child2.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.material = whiteMat;
            }
            var seq = DOTween.Sequence();
            seq.Append(child.DOShakePosition(0.2f, 0.6f, 30));
            seq.Append(child.transform.DOScale(Vector3.zero, 0.15f));
            seq.AppendCallback(() =>
            {
                SoundManager.Instance.PlaySoundEffect("eraser_despawn");
                foreach (Transform child3 in child)
                {
                    var newParticles = Instantiate(eraserParticles, child.position, Quaternion.identity);
                    Destroy(newParticles, 1f);
                }
            });
        }
        var seq2 = DOTween.Sequence();
        seq2.AppendInterval(ind * 0.1f + 1f);
        seq2.AppendCallback(() =>
        {
            //UpgradeManager.Instance.SpawnUpgrades();
        });
    }
}
