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
    // Existing animation timing constants
    private const float ANIMATION_FRAME_DURATION = 0.2f;
    private const float SPAWN_SCALE_DURATION = 0.25f;
    private const float COLLECT_MOVE_DURATION = 0.4f;
    private const float COLLECT_PUNCH_DURATION = 0.2f;
    private const float COLLECT_SCALE_DURATION = 0.2f;
    private const int TOTAL_FRAMES = 4;

    // New spawn arc animation constants
    private const float SPAWN_ARC_DURATION = 0.5f;
    private const float SPAWN_ARC_HEIGHT = 1.5f;
    private const float SPAWN_HORIZONTAL_DISTANCE = 0.5f;

    // internal state tracking
    private float animationTimer = ANIMATION_FRAME_DURATION;
    private int currentFrame;
    private float coinValue;
    private List<Sprite> coinSprites = new();
    private SpriteRenderer spriteRenderer;
    private Vector3 targetPosition;

    private Sequence activeSequence;

    // serialized fields
    [SerializeField] private Material whiteMat;

    public void InitializeCoin(CoinStruct coin)
    {
        SetupCoinProperties(coin);
        transform.localScale = Vector2.zero;
        // Store the target position (where we want the coin to land)
        targetPosition = transform.position;
        // Offset the initial position slightly to the left or right randomly
        float randomDirection = Random.Range(0f, 1f) > 0.5f ? 1f : -1f;
        transform.position += Vector3.right * SPAWN_HORIZONTAL_DISTANCE * randomDirection;
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
        // Kill any existing sequence
        activeSequence?.Kill();

        // Scale up from zero
        transform.DOScale(Vector2.one, SPAWN_SCALE_DURATION);

        // Create the arc jump sequence
        activeSequence = DOTween.Sequence();

        // Create the arc movement
        activeSequence.Append(transform.DOPath(
            CreateArcPath(),
            SPAWN_ARC_DURATION,
            PathType.CatmullRom
        ).SetEase(Ease.OutQuad));

        // Add a small bounce at the end
        activeSequence.Append(transform.DOPunchPosition(Vector2.up * 0.2f, 0.2f, 1, 0));
    }

    private Vector3[] CreateArcPath()
    {
        Vector3[] path = new Vector3[3];
        path[0] = transform.position;
        path[1] = Vector3.Lerp(transform.position, targetPosition, 0.5f) + Vector3.up * SPAWN_ARC_HEIGHT;
        path[2] = targetPosition;
        return path;
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
        // Kill any existing sequence
        activeSequence?.Kill();

        activeSequence = DOTween.Sequence();
        activeSequence.Append(transform.DOPunchScale(Vector2.one * 0.25f, COLLECT_PUNCH_DURATION));
        activeSequence.Append(transform.DOScale(Vector2.zero, COLLECT_SCALE_DURATION));
        activeSequence.AppendCallback(OnCollectComplete);
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
        // Kill all tweens associated with this transform
        transform.DOKill();
        // Kill the active sequence if it exists
        activeSequence?.Kill();
    }
}