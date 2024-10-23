using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    // serialized fields for unity inspector
    [SerializeField] private Slider attackSlider;
    [SerializeField] private GameObject projectile;
    [SerializeField] private List<Color> colors = new();
    [SerializeField] private DragController rubberRender;

    // private fields for internal state management
    private Transform launchPoint;
    private float sliderValue;
    private float attackPower;
    private bool attacking;
    private int attackState;
    private Color currentColor = Color.white;

    // constants
    private const float ATTACK_MAX = 2.5f;
    private const float SLIDER_LERP_SPEED = 2.5f;
    private const float PROJECTILE_BASE_FORCE = 300f;

    // public properties
    public bool Attacking => attacking;
    public Color CurrentColor => currentColor;
    public static PlayerAttack Instance { get; private set; }

    // events
    public static event Action OnAttackInitiate;
    public static event Action OnAttackHalt;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        InitializeComponents();
    }

    private void Update()
    {
        UpdateAttackState();
        HandlePlayerInput();
        UpdateAttackPower();
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
        // update slider value with smooth interpolation
        sliderValue = Mathf.Clamp01(Mathf.Lerp(sliderValue, attackPower, Time.deltaTime * SLIDER_LERP_SPEED));
        attackSlider.value = sliderValue;

        // update attack state and colors
        attackState = Mathf.FloorToInt(sliderValue * 4);
        UpdateColors();
    }

    private void UpdateColors()
    {
        // update current color and slider fill color
        currentColor = AttackStateToColor(attackState);
        var sliderFill = attackSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        sliderFill.color = currentColor;
    }

    private void HandlePlayerInput()
    {
        // handle mouse input for attack controls
        if (Input.GetMouseButtonDown(0))
        {
            AttackInitiate();
        }
        else if (Input.GetMouseButtonUp(0) && attacking)
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
        // check if attack can be initiated
        if (GameManager.Instance.BetweenRounds) return;

        attacking = true;
        SoundManager.Instance.PlaySoundEffect("band_pull");
        OnAttackInitiate?.Invoke();
    }

    private void AttackHalt()
    {
        OnAttackHalt?.Invoke();
        PlayAttackSoundEffect();
        SpawnProjectile();
        ResetAttackState();
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
        // only apply force if not in highest attack state
        if (attackState != 3)
        {
            var rb = projectileObj.GetComponent<Rigidbody2D>();
            var direction = -PlayerMovement.Instance.GetDirectionToMouse();
            rb.AddForce(direction * PROJECTILE_BASE_FORCE * attackPower);
        }
    }

    private void RotateProjectile(GameObject projectileObj)
    {
        var direction = PlayerMovement.Instance.GetDirectionToMouse();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectileObj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void ResetAttackState()
    {
        attacking = false;
        attackPower = 0;
    }

    // utility methods
    public Color AttackStateToColor(int state)
    {
        // convert attack state to corresponding color
        if (state == 0 || state >= colors.Count)
        {
            return colors[0];
        }
        return colors[state];
    }
}