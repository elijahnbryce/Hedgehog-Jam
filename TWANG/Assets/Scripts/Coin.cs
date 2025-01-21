using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private int currentFrame;
    private float coinValue;
    private float animationTimer;
    private List<Sprite> coinSprites = new();
    private SpriteRenderer spriteRenderer;
    private Vector3 targetPosition;
    private PlayerMovement pm;
    private Sequence activeSequence;
    private bool collected = false;

    [SerializeField] private Material whiteMat;

    public void InitializeCoin(CoinStruct coin)
    {
        SetupCoinProperties(coin);
        transform.localScale = Vector2.zero;
        targetPosition = transform.position;
        float randomDirection = Random.Range(0f, 1f) > 0.5f ? 1f : -1f;
        transform.position += Vector3.right * 0.5f * randomDirection;
        StartCoroutine(nameof(PlaySpawnAnimationCoroutine));
    }

    private void SetupCoinProperties(CoinStruct coin)
    {
        coinValue = (int)coin.Type / 100f;
        coinSprites = coin.Sprites;
        currentFrame = Random.Range(0, coinSprites.Count);
    }

    private IEnumerator PlaySpawnAnimationCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.25f));
        PlaySpawnAnimation();
    }

    private void PlaySpawnAnimation()
    {
        activeSequence?.Kill();
        transform.DOScale(Vector2.one, 0.25f);
        activeSequence = DOTween.Sequence();
        activeSequence.Append(transform.DOPath(CreateArcPath(), 0.5f, PathType.CatmullRom).SetEase(Ease.OutQuad));
        activeSequence.Append(transform.DOPunchPosition(Vector2.up * 0.2f, 0.2f, 1, 0));
    }

    private Vector3[] CreateArcPath()
    {
        Vector3[] path = new Vector3[3];
        path[0] = transform.position;
        path[1] = Vector3.Lerp(transform.position, targetPosition, 0.5f) + Vector3.up * 1.5f;
        path[2] = targetPosition;
        return path;
    }

    private void Start()
    {
        InitializeSpriteRenderer();
        pm = PlayerMovement.Instance;
    }

    private void InitializeSpriteRenderer()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!collected)
        {
            UpdateCoinAnimation();
            CheckPlayerProximity();
        }
    }

    private void CheckPlayerProximity()
    {
        if (collected) return; 

        float distance = Vector3.Distance(transform.position, pm.PlayerPosition);
        if (distance <= 4.5f)
        {
            Vector3 direction = ((Vector3)pm.PlayerPosition - transform.position).normalized;
            transform.position += direction * Time.deltaTime * 5f;

            if (distance <= 0.5f)  
            {
                ClaimCoin();
            }
        }
    }

    private void UpdateCoinAnimation()
    {
        animationTimer -= Time.deltaTime;
        if (animationTimer < 0)
        {
            animationTimer = 0.2f;
            currentFrame = (currentFrame + 1) % 4;
        }
        spriteRenderer.sprite = coinSprites[currentFrame];
    }

    public void ClaimCoin()
    {
        if (collected) return;
        collected = true;

        activeSequence?.Kill();

        DisableCollision();
        SetCollectMaterial();
        PlayCollectAnimation();
    }

    private void DisableCollision()
    {
        Destroy(GetComponent<Collider2D>());
    }

    private void SetCollectMaterial()
    {
        spriteRenderer.material = whiteMat;
    }

    private void PlayCollectAnimation()
    {
        CreateCollectSequence();
    }

    private void CreateCollectSequence()
    {
        activeSequence = DOTween.Sequence();
        activeSequence.Append(transform.DOMove(transform.position + Vector3.up, 0.2f)); 
        activeSequence.Append(transform.DOPunchScale(Vector2.one * 0.25f, 0.2f));
        activeSequence.Append(transform.DOScale(Vector2.zero, 0.2f));
        activeSequence.OnComplete(OnCollectComplete);  
    }

    private void OnCollectComplete()
    {
        Destroy(gameObject);
        PlayCollectSound();
        UpdateGameScore();
    }

    private void PlayCollectSound()
    {
        SoundManager.Instance.PlaySoundEffect("coin_pickup");
    }

    private void UpdateGameScore()
    {
        Debug.Log($"Added {coinValue} to score");
        GameManager.Instance.UpdateScore(coinValue);
    }

    private void OnDestroy()
    {
        transform.DOKill();
        activeSequence?.Kill();
    }
}