using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manages the spawning, despawning, and behavior of eraser configurations in the game
/// Implements the singleton pattern for global access
/// </summary>
public class EraserManager : MonoBehaviour
{
    // Animation timing constants
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

    // Serialized fields for configuration
    [SerializeField] private List<Sprite> sprites = new();
    [SerializeField] private List<Sprite> colliderSprites = new();
    [SerializeField] private List<Sprite> crackedSprites = new();
    [SerializeField] private List<GameObject> configs = new();
    [SerializeField] private Material whiteMat, defaultMat;
    [SerializeField] private ParticleSystem eraserParticles;

    // Sequence tracking
    private List<Sequence> activeSpawnSequences = new List<Sequence>();
    private List<Sequence> activeUnspawnSequences = new List<Sequence>();

    // Singleton instance
    public static EraserManager Instance { get; private set; }

    private void Awake()
    {
        InitializeSingleton();
    }

    private void OnDestroy()
    {
        // Clean up all active sequences
        KillAllSequences();
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

    // Public accessors for sprite collections
    public Sprite GetColliderSprite(int index) => colliderSprites[index];
    public Sprite GetCrackedSprite(int index) => crackedSprites[index];

    /// <summary>
    /// Spawns a new random eraser configuration
    /// </summary>
    public void SpawnConfig()
    {
        Debug.Log("Spawn Config");
        KillAllSequences(); 
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
            if (child != null)
            {
                Destroy(child.gameObject);
            }
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
        if (chosenConfig == null) return null;

        GameObject configInstance = Instantiate(chosenConfig, transform);
        configInstance.transform.localPosition = Vector3.up * SPAWN_HEIGHT_OFFSET;
        return configInstance;
    }

    private void AnimateConfigPieces(Transform configTransform, List<bool> activationStates)
    {
        if (configTransform == null) return;

        for (int i = 0; i < configTransform.childCount; i++)
        {
            Transform piece = configTransform.GetChild(i);
            if (piece != null)
            {
                piece.gameObject.SetActive(activationStates[i]);
                CreateSpawnSequence(piece, i);
            }
        }
    }

    private void CreateSpawnSequence(Transform piece, int index)
    {
        if (piece == null) return;

        Vector3 targetPosition = piece.position - Vector3.up * SPAWN_HEIGHT_OFFSET;

        Sequence sequence = DOTween.Sequence();
        sequence
            .SetAutoKill(true)
            .OnKill(() => activeSpawnSequences.Remove(sequence));

        sequence.AppendInterval(index * SPAWN_DELAY_PER_PIECE);
        sequence.Append(piece.DOMove(targetPosition, SPAWN_MOVE_DURATION)
            .SetEase(Ease.OutQuad));
        sequence.AppendCallback(() => { if (piece != null) OnPieceSpawnAnimationComplete(piece); });
        sequence.AppendInterval(SPAWN_DELAY_PER_PIECE);
        sequence.AppendCallback(() => { if (piece != null) OnPieceSpawnFinished(piece); });

        activeSpawnSequences.Add(sequence);
    }

    private void OnPieceSpawnAnimationComplete(Transform piece)
    {
        if (piece == null) return;

        SetPieceMaterial(piece, whiteMat);

        Sequence punchSequence = DOTween.Sequence();
        punchSequence
            .SetAutoKill(true)
            .OnKill(() => activeSpawnSequences.Remove(punchSequence));

        punchSequence.Append(piece.DOPunchScale(Vector2.one * SPAWN_PUNCH_SCALE, SPAWN_PUNCH_DURATION)
            .SetEase(Ease.OutQuad));
        punchSequence.AppendCallback(() => { if (piece != null) PlaySpawnSound(); });

        activeSpawnSequences.Add(punchSequence);
    }

    private void OnPieceSpawnFinished(Transform piece)
    {
        if (piece == null) return;

        SetPieceMaterial(piece, defaultMat);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BetweenRounds = false;
        }
    }

    private void SetPieceMaterial(Transform piece, Material material)
    {
        if (piece == null) return;

        foreach (Transform child in piece)
        {
            if (child == null) continue;

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
    /// Generates a list of boolean values for piece activation
    /// Ensures at least half the pieces are always active
    /// </summary>
    private List<bool> GenerateActivationStates(int amount)
    {
        var states = new List<bool>();

        // Guarantee half the pieces are active
        int guaranteedActive = amount / 2;
        for (int i = 0; i < guaranteedActive; i++)
        {
            states.Add(true);
        }

        // Randomize remaining pieces
        while (states.Count < amount)
        {
            states.Add(Random.value > 0.5f);
        }

        HelperClass.Shuffle(states);
        return states;
    }

    /// <summary>
    /// Unspawns the current eraser configuration with animations
    /// </summary>
    public void UnspawnConfig()
    {
        KillAllSequences(); // Ensure clean slate before unspawning

        if (GameManager.Instance != null)
        {
            GameManager.Instance.BetweenRounds = true;
        }

        int pieceCount = 0;
        foreach (Transform piece in transform)
        {
            if (piece != null)
            {
                pieceCount++;
                CreateUnspawnSequence(piece);
            }
        }

        CreatePostUnspawnSequence(pieceCount);
    }

    private void CreateUnspawnSequence(Transform piece)
    {
        if (piece == null) return;

        SetPieceMaterial(piece, whiteMat);

        Sequence sequence = DOTween.Sequence();
        sequence
            .SetAutoKill(true)
            .OnKill(() => activeUnspawnSequences.Remove(sequence));

        sequence.Append(piece.DOShakePosition(UNSPAWN_SHAKE_DURATION, UNSPAWN_SHAKE_STRENGTH, (int)UNSPAWN_SHAKE_VIBRATO)
            .SetEase(Ease.OutQuad));
        sequence.Append(piece.transform.DOScale(Vector3.zero, UNSPAWN_SCALE_DURATION)
            .SetEase(Ease.InQuad));
        sequence.AppendCallback(() => { if (piece != null) OnPieceUnspawnComplete(piece); });

        activeUnspawnSequences.Add(sequence);
    }

    private void OnPieceUnspawnComplete(Transform piece)
    {
        if (piece == null) return;

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
        if (piece == null || eraserParticles == null) return;

        foreach (Transform child in piece)
        {
            if (child != null)
            {
                var particles = Instantiate(eraserParticles, piece.position, Quaternion.identity);
                if (particles != null)
                {
                    Destroy(particles.gameObject, PARTICLE_LIFETIME);
                }
            }
        }
    }

    private void CreatePostUnspawnSequence(int pieceCount)
    {
        float totalDuration = pieceCount * SPAWN_DELAY_PER_PIECE + SPAWN_MOVE_DURATION;

        Sequence sequence = DOTween.Sequence();
        sequence
            .SetAutoKill(true)
            .OnKill(() => activeUnspawnSequences.Remove(sequence));

        sequence.AppendInterval(totalDuration);
        sequence.AppendCallback(OnUnspawnComplete);

        activeUnspawnSequences.Add(sequence);
    }

    private void OnUnspawnComplete()
    {
        // Add any post-unspawn logic here
        //UpgradeManager.Instance.SpawnUpgrades();
    }

    // Clean up method for sequences
    public void KillAllSequences()
    {
        foreach (var sequence in activeSpawnSequences.ToList())
        {
            sequence?.Kill();
        }
        foreach (var sequence in activeUnspawnSequences.ToList())
        {
            sequence?.Kill();
        }
        activeSpawnSequences.Clear();
        activeUnspawnSequences.Clear();
    }
}