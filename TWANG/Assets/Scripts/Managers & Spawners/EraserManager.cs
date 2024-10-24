using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// manages the spawning, despawning, and behavior of eraser configurations in the game
/// implements the singleton pattern for global access
/// </summary>
public class EraserManager : MonoBehaviour
{
    // animation timing constants
    private const float SPAWN_HEIGHT_OFFSET = 20f;
    private const float SPAWN_DELAY_PER_PIECE = 0.1f;
    private const float SPAWN_MOVE_DURATION = 1f;
    private const float SPAWN_PUNCH_SCALE = 0.1f;
    private const float SPAWN_PUNCH_DURATION = 0.1f;
    private const float UNSPAWN_SHAKE_DURATION = 0.2f;
    private const float UNSPAWN_SHAKE_STRENGTH = 0.6f;
    private const float UNSPAWN_SHAKE_VIBRATO = 30f;
    private const float UNSPAWN_SCALE_DURATION = 0.15f;
    private const float PARTICLE_LIFETIME = 1f;

    // serialized fields for configuration
    [SerializeField] private List<Sprite> sprites = new();
    [SerializeField] private List<Sprite> colliderSprites = new();
    [SerializeField] private List<Sprite> crackedSprites = new();
    [SerializeField] private List<GameObject> configs = new();
    [SerializeField] private Material whiteMat, defaultMat;
    [SerializeField] private ParticleSystem eraserParticles;

    // singleton instance
    public static EraserManager Instance { get; private set; }

    private void Awake()
    {
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // public accessors for sprite collections
    public Sprite GetColliderSprite(int index) => colliderSprites[index];
    public Sprite GetCrackedSprite(int index) => crackedSprites[index];

    /// <summary>
    /// spawns a new random eraser configuration
    /// </summary>
    public void SpawnConfig()
    {
        ClearExistingConfig();
        GameObject configObject = SpawnRandomConfig();
        if (configObject == null) return;

        Transform configTransform = configObject.transform;
        List<bool> activationStates = GenerateActivationStates(configTransform.childCount);
        AnimateConfigPieces(configTransform, activationStates);
    }

    private void ClearExistingConfig()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private GameObject SpawnRandomConfig()
    {
        if (configs.Count == 0)
        {
            Debug.LogError("No configs available in EraserManager.");
            return null;
        }

        GameObject chosenConfig = configs[Random.Range(0, configs.Count)];
        GameObject configInstance = Instantiate(chosenConfig, transform);
        configInstance.transform.localPosition = Vector3.up * SPAWN_HEIGHT_OFFSET;
        return configInstance;
    }

    private void AnimateConfigPieces(Transform configTransform, List<bool> activationStates)
    {
        for (int i = 0; i < configTransform.childCount; i++)
        {
            Transform piece = configTransform.GetChild(i);
            piece.gameObject.SetActive(activationStates[i]);
            CreateSpawnSequence(piece, i);
        }
    }

    private void CreateSpawnSequence(Transform piece, int index)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(index * SPAWN_DELAY_PER_PIECE);
        sequence.Append(piece.DOMove(piece.position - Vector3.up * SPAWN_HEIGHT_OFFSET, SPAWN_MOVE_DURATION));
        sequence.AppendCallback(() => OnPieceSpawnAnimationComplete(piece));
        sequence.AppendInterval(SPAWN_DELAY_PER_PIECE);
        sequence.AppendCallback(() => OnPieceSpawnFinished(piece));
    }

    private void OnPieceSpawnAnimationComplete(Transform piece)
    {
        SetPieceMaterial(piece, whiteMat);
        piece.DOPunchScale(Vector2.one * SPAWN_PUNCH_SCALE, SPAWN_PUNCH_DURATION);
        PlaySpawnSound();
    }

    private void OnPieceSpawnFinished(Transform piece)
    {
        SetPieceMaterial(piece, defaultMat);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BetweenRounds = false;
        }
    }

    private void SetPieceMaterial(Transform piece, Material material)
    {
        foreach (Transform child in piece)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.material = material;
            }
        }
    }

    private void PlaySpawnSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySoundEffect("eraser_spawn");
        }
    }

    /// <summary>
    /// generates a list of boolean values for piece activation
    /// ensures at least half the pieces are always active
    /// </summary>
    private List<bool> GenerateActivationStates(int amount)
    {
        var states = new List<bool>();

        // guarantee half the pieces are active
        int guaranteedActive = amount / 2;
        for (int i = 0; i < guaranteedActive; i++)
        {
            states.Add(true);
        }

        // randomize remaining pieces
        while (states.Count < amount)
        {
            states.Add(Random.value > 0.5f);
        }

        HelperClass.Shuffle(states);
        return states;
    }

    /// <summary>
    /// unspawns the current eraser configuration with animations
    /// </summary>
    public void UnspawnConfig()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BetweenRounds = true;
        }

        int pieceCount = 0;
        foreach (Transform piece in transform)
        {
            pieceCount++;
            CreateUnspawnSequence(piece);
        }

        CreatePostUnspawnSequence(pieceCount);
    }

    private void CreateUnspawnSequence(Transform piece)
    {
        SetPieceMaterial(piece, whiteMat);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(piece.DOShakePosition(UNSPAWN_SHAKE_DURATION, UNSPAWN_SHAKE_STRENGTH, (int)UNSPAWN_SHAKE_VIBRATO));
        sequence.Append(piece.transform.DOScale(Vector3.zero, UNSPAWN_SCALE_DURATION));
        sequence.AppendCallback(() => OnPieceUnspawnComplete(piece));
    }

    private void OnPieceUnspawnComplete(Transform piece)
    {
        PlayUnspawnSound();
        SpawnUnspawnParticles(piece);
    }

    private void PlayUnspawnSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySoundEffect("eraser_despawn");
        }
    }

    private void SpawnUnspawnParticles(Transform piece)
    {
        foreach (Transform child in piece)
        {
            var particles = Instantiate(eraserParticles, piece.position, Quaternion.identity);
            Destroy(particles.gameObject, PARTICLE_LIFETIME);
        }
    }

    private void CreatePostUnspawnSequence(int pieceCount)
    {
        float totalDuration = pieceCount * SPAWN_DELAY_PER_PIECE + SPAWN_MOVE_DURATION;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(totalDuration);
        sequence.AppendCallback(OnUnspawnComplete);
    }

    private void OnUnspawnComplete()
    {
        //UpgradeManager.Instance.SpawnUpgrades();
    }
}