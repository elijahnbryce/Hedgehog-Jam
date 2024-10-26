using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEditor.Build.Player;
using UnityEngine;

[System.Serializable]
public struct DirectionalFrames
{
    public List<Sprite> Sprites;
}

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private float timeBetweenFrames = 0.2f;
    [SerializeField] private SpriteRenderer primaryHandSR;
    [SerializeField] private SpriteRenderer secondaryHandSR;
    [SerializeField] private List<Sprite> primaryHandSprites = new();
    [SerializeField] private List<Sprite> primaryHandGrabSprites = new();
    [SerializeField] private List<Sprite> secondaryHandSprites = new();
    [SerializeField] private List<Sprite> secondaryHoldingSprites = new();
    [SerializeField] private List<Sprite> secondaryHandGrabSprites = new();

    // private state fields
    private float timer;
    private int frame;
    private int frameMax = 2;
    private int animationIndex = 0;
    private int hurtFrame = 0;

    // constants
    private static readonly string[] DIRECTION_NAMES = { "NW", "N", "NE", "E", "SE", "S", "SW", "W" };
    private static readonly int[] DIRECTION_REMAPPING = { 7, 6, 5, 4, 3, 2, 1, 0 };
    private const int SPRITE_VARIANTS_PER_DIRECTION = 6;
    private const int MAX_HURT_FRAMES = 2;
    private const float DEGREES_PER_DIRECTION = 45f;

    // singleton instance
    public static PlayerAnimation Instance { get; private set; }

    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        RegisterEventHandlers();
    }

    private void Update()
    {
        UpdateAnimationTimers();
        UpdateHandSprites();
    }

    // initialization methods
    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void RegisterEventHandlers()
    {
        PlayerAttack.OnAttackInitiate += AttackStart;
        PlayerAttack.OnAttackHalt += AttackEnd;
    }

    // update methods
    private void UpdateAnimationTimers()
    {
        timer += Time.deltaTime;
        if (timer > timeBetweenFrames)
        {
            UpdateFrameCounters();
        }
    }

    private void UpdateFrameCounters()
    {
        timer = 0;
        hurtFrame = (hurtFrame + 1) % (MAX_HURT_FRAMES + 1);
        // frame = (frame + 1) % frameMax; // uncomment when more frames are available
    }

    private void UpdateHandSprites()
    {
        UpdatePrimaryHandSprite();
        UpdateSecondaryHandSprite();
    }

    // sprite update methods
    private void UpdatePrimaryHandSprite()
    {
        //Vector2 primaryDir = PlayerMovement.Instance.GetDirectionToMouse(true);

        var primaryDir = PlayerMovement.Instance.GetDirectionOfPrimaryHand();

        int spriteIndex = CalculatePrimaryHandSpriteIndex(primaryDir);

        primaryHandSR.sprite = PlayerAttack.Instance.Attacking
            ? primaryHandGrabSprites[spriteIndex]
            : primaryHandSprites[spriteIndex];
    }

    private int CalculatePrimaryHandSpriteIndex(Vector2 direction)
    {
        int directionIndex = GetDirection(direction);
        int hurtState = 4 - GameManager.Instance.health;
        int hurtFrameOffset = (hurtState == 3) ? hurtFrame : 0;

        return SPRITE_VARIANTS_PER_DIRECTION * directionIndex + hurtState + hurtFrameOffset;
    }

    private void UpdateSecondaryHandSprite()
    {
        if (GameManager.Instance.BetweenRounds)
        {
            secondaryHandSR.sprite = null;
            return;
        }

        Vector2 secondaryDir = PlayerMovement.Instance.GetDirectionToPrimaryHand();
        int directionIndex = GetDirection(-secondaryDir);

        if (PlayerAttack.Instance.Attacking)
        {
            secondaryHandSR.sprite = secondaryHandGrabSprites[directionIndex];
        }
        else
        {
            secondaryHandSR.sprite = PlayerAttack.Instance.HasBand
             ? secondaryHoldingSprites[directionIndex]
             : secondaryHandSprites[directionIndex];
        }
    }

    // direction calculation methods
    public int GetDirection(Vector2 dir)
    {
        float angle = CalculateAngleFromVector(dir);
        return MapAngleToDirection(angle);
    }

    private float CalculateAngleFromVector(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle < 0 ? angle + 360 : angle;
    }

    private int MapAngleToDirection(float angle)
    {
        int directionIndex = Mathf.RoundToInt(angle / DEGREES_PER_DIRECTION) % 8;
        return DIRECTION_REMAPPING[directionIndex];
    }

    // animation state handlers
    private void AttackStart()
    {
        animationIndex = 1;
    }

    private void AttackEnd()
    {
        animationIndex = 0;
    }
}