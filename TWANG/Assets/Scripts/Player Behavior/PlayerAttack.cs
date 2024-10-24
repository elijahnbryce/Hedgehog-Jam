using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private Slider attackSlider;
    [SerializeField] private GameObject projectile;
    [SerializeField] private List<Color> colors = new();
    [SerializeField] private DragController rubberRender;
    [SerializeField] private Transform primaryHand;    // Reference to the main player/hand
    [SerializeField] private Transform secondaryHand;  // Reference to the secondary hand
    [SerializeField] private float maxStretchDistance = 6f;  // Maximum distance the band can stretch
    [SerializeField] private float minStretchDistance = 0.5f;

    // private fields for internal state management
    private Transform launchPoint;
    private float sliderValue;
    private float attackPower;
    private bool attacking;
    private int attackState;
    private Color currentColor = Color.white;
    private Vector2 attackDirection;

    // constants
    private const float ATTACK_MAX = 2.5f;
    private const float SLIDER_LERP_SPEED = 2.5f;
    private const float PROJECTILE_BASE_FORCE = 300f;
    private const float MIN_ATTACK_POWER = 0.2f;

    // public properties
    public bool Attacking => attacking;
    public Color CurrentColor => currentColor;
    public static PlayerAttack Instance { get; private set; }

    // events
    public static event Action OnAttackInitiate;
    public static event Action OnAttackHalt;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeComponents();
    }

    private void Update()
    {
        if (!GameManager.Instance.BetweenRounds)
        {
            HandleAttackInput();
            if (attacking)
            {
                UpdateAttackState();
            }
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackInitiate();
        }
        else if (Input.GetKeyUp(KeyCode.Space) && attacking)
        {
            AttackHalt();
        }
    }

    // initialization methods
    private void InitializeSingleton()
    {
        // handle singleton pattern setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void InitializeComponents()
    {
        // find and setup required components
        launchPoint = transform.GetChild(0).GetChild(0);
    }

    // update methods
    private void UpdateAttackState()
    {
        // Calculate attack power based on distance between hands
        Vector2 handsDelta = secondaryHand.position - primaryHand.position;
        float distance = handsDelta.magnitude;

        // Normalize distance to 0-1 range based on max stretch
        attackPower = Mathf.Clamp01(distance / maxStretchDistance);

        // Update attack direction (will be used when firing)
        attackDirection = handsDelta.normalized;

        // Update slider and visual feedback
        sliderValue = Mathf.Lerp(sliderValue, attackPower, Time.deltaTime * SLIDER_LERP_SPEED);
        attackSlider.value = sliderValue;

        // Update attack state and colors
        attackState = Mathf.FloorToInt(sliderValue * 4);
        UpdateColors();

        // Update rubber band visual
        rubberRender.UpdateBand(attackPower);
    }

    private void UpdateColors()
    {
        currentColor = AttackStateToColor(attackState);
        var sliderFill = attackSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        sliderFill.color = currentColor;
    }

    private void HandlePlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackInitiate();
        }
        else if (Input.GetKeyUp(KeyCode.Space) && attacking)
        {
            AttackHalt();
        }
    }

    private void UpdateAttackPower()
    {
        // update attack power while attacking
        if (!attacking) return;

        if (attackPower < ATTACK_MAX)
        {
            attackPower = PlayerMovement.Instance.GetAttackPower();
            rubberRender.UpdateBand(attackPower / ATTACK_MAX);
            return;
        }

        AttackHalt();
    }

    // attack handling methods
    private void AttackInitiate()
    {
        if (GameManager.Instance.BetweenRounds) return;

        attacking = true;
        attackPower = 0;
        sliderValue = 0;
        SoundManager.Instance.PlaySoundEffect("band_pull");
        OnAttackInitiate?.Invoke();
    }

    private void AttackHalt()
    {
        // Only fire if we've pulled back enough
        if (attackPower >= MIN_ATTACK_POWER)
        {
            FireProjectile();
        }

        OnAttackHalt?.Invoke();
        SoundManager.Instance.PlaySoundEffect("band_release");
        ResetAttackState();
    }

    private void FireProjectile()
    {
        // Calculate firing direction (from secondary hand to primary hand)
        Vector2 fireDirection = (primaryHand.position - secondaryHand.position).normalized;

        // Create and setup projectile
        GameObject newProjectile = Instantiate(projectile, secondaryHand.position, Quaternion.identity);

        // Configure visuals
        var projectileSprite = newProjectile.transform.GetChild(0).GetComponent<SpriteRenderer>();
        projectileSprite.color = AttackStateToColor(attackState);

        // Initialize rubber band component
        newProjectile.GetComponent<RubberBand>().InitializeProjectile(
            attackState,
            PlayerMovement.Instance.FacingDir
        );

        // Apply physics
        var rb = newProjectile.GetComponent<Rigidbody2D>();
        rb.AddForce(fireDirection * PROJECTILE_BASE_FORCE * attackPower);

        // Rotate projectile to face direction of travel
        float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
        newProjectile.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void PlayAttackSoundEffect()
    {
        SoundManager.Instance.PlaySoundEffect("band_release");
    }

    private void SpawnProjectile()
    {
        // create and setup new projectile
        var newProjectile = CreateProjectile();
        ConfigureProjectileVisuals(newProjectile);
        ConfigureProjectilePhysics(newProjectile);
        RotateProjectile(newProjectile);
    }

    private GameObject CreateProjectile()
    {
        return Instantiate(projectile, launchPoint.position, Quaternion.identity);
    }

    private void ConfigureProjectileVisuals(GameObject projectileObj)
    {
        var projectileSprite = projectileObj.transform.GetChild(0).GetComponent<SpriteRenderer>();
        projectileSprite.color = AttackStateToColor(attackState);
        projectileObj.GetComponent<RubberBand>().InitializeProjectile(attackState, PlayerMovement.Instance.FacingDir);
    }

    private void ConfigureProjectilePhysics(GameObject projectileObj)
    {
        var rb = projectileObj.GetComponent<Rigidbody2D>();
        //var direction = -PlayerMovement.Instance.GetDirectionToMouse();
        var direction = -PlayerMovement.Instance.GetDirectionOfPrimaryHand();

        rb.AddForce(direction * PROJECTILE_BASE_FORCE * attackPower);
    }

    private void RotateProjectile(GameObject projectileObj)
    {
        //var direction = PlayerMovement.Instance.GetDirectionToMouse();
        var direction = PlayerMovement.Instance.GetDirectionOfPrimaryHand();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectileObj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void ResetAttackState()
    {
        attacking = false;
        attackPower = 0;
        sliderValue = 0;
        attackDirection = Vector2.zero;
    }

    // utility methods
    public Color AttackStateToColor(int state)
    {
        return state == 0 || state >= colors.Count ? colors[0] : colors[state];
    }

    public float GetCurrentStretch()
    {
        return attackPower;
    }
}