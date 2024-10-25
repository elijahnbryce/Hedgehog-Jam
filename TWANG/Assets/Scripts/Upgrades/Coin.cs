using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// handles the behavior and animation of collectible coins in the game
/// </summary>
public class Coin : MonoBehaviour
{
    // animation timing constants
    private const float ANIMATION_FRAME_DURATION = 0.2f;
    private const float SPAWN_SCALE_DURATION = 0.25f;
    private const float COLLECT_MOVE_DURATION = 0.4f;
    private const float COLLECT_PUNCH_DURATION = 0.2f;
    private const float COLLECT_SCALE_DURATION = 0.2f;
    private const int TOTAL_FRAMES = 4;

    // internal state tracking
    private float animationTimer = ANIMATION_FRAME_DURATION;
    private int currentFrame;
    private int coinValue;
    private List<Sprite> coinSprites = new();
    private SpriteRenderer spriteRenderer;

    // serialized fields
    [SerializeField] private Material whiteMat;

    // todo: replace with scriptableobject implementation
    public void InitializeCoin(CoinStruct coin)
    {
        SetupCoinProperties(coin);
        transform.localScale = Vector2.zero;
        StartCoroutine(nameof(PlaySpawnAnimationCoroutine));
    }

    private void SetupCoinProperties(CoinStruct coin)
    {
        coinValue = (int)coin.Type;
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
        transform.DOScale(Vector2.one, SPAWN_SCALE_DURATION);
        transform.DOPunchPosition(Vector2.up / 2, SPAWN_SCALE_DURATION);
    }

    private void Start()
    {
        InitializeSpriteRenderer();
    }

    private void InitializeSpriteRenderer()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        UpdateCoinAnimation();
    }

    private void UpdateCoinAnimation()
    {
        UpdateAnimationTimer();
        UpdateSpriteFrame();
    }

    private void UpdateAnimationTimer()
    {
        animationTimer -= Time.deltaTime;
        if (animationTimer < 0)
        {
            animationTimer = ANIMATION_FRAME_DURATION;
            AdvanceAnimationFrame();
        }
    }

    private void AdvanceAnimationFrame()
    {
        currentFrame = (currentFrame + 1) % TOTAL_FRAMES;
    }

    private void UpdateSpriteFrame()
    {
        spriteRenderer.sprite = coinSprites[currentFrame];
    }

    public void ClaimCoin()
    {
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
        // animate upward movement
        transform.DOMove(transform.position + Vector3.up, COLLECT_MOVE_DURATION);

        // create collection animation sequence
        CreateCollectSequence();
    }

    private void CreateCollectSequence()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOPunchScale(Vector2.one * 0.25f, COLLECT_PUNCH_DURATION));
        sequence.Append(transform.DOScale(Vector2.zero, COLLECT_SCALE_DURATION));
        sequence.AppendCallback(OnCollectComplete);
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
        GameManager.Instance.UpdateScore(coinValue);
    }
}