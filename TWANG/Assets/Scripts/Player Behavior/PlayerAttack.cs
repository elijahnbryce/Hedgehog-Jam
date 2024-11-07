using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private Slider attackSlider;
    [SerializeField] private GameObject projectile;
    [SerializeField] private DragController rubberRender;
    [SerializeField] private Transform primaryHand;
    [SerializeField] private Transform secondaryHand;
    [SerializeField] private float maxStretchDistance = 6f;
    private bool hasBand = true;
    public bool HasBand { get { return hasBand; } }

    private float sliderValue;
    private float attackPower;
    private bool attacking;
    private int attackState;
    private Color currentColor = Color.white;

    private const float ATTACK_MAX = 2.5f;
    private const float SLIDER_LERP_SPEED = 2.5f;
    private const float PROJECTILE_BASE_FORCE = 1000f;
    private const float MIN_ATTACK_POWER = 0.2f;

    public bool Attacking => attacking;
    public Color CurrentColor => currentColor;
    public static PlayerAttack Instance { get; private set; }

    public static event Action OnAttackInitiate;
    public static event Action OnAttackHalt;

    private float attackCooldown = 0f;
    private float attackCooldownTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
		if (!GameManager.Instance.GameRunning)
			return;

        HandleAttackInput();
		if (attacking)
		{
			UpdateAttackState();
		}
    }

	private void OnDestroy()
	{
		Instance = null;
		OnAttackInitiate = null;
		OnAttackHalt = null;
	}

	private void HandleAttackInput()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackInitiate();
        }
        else if (Input.GetKeyUp(KeyCode.Space) && attacking)
        {
            AttackHalt();
        }
    }

    private void UpdateAttackState()
    {
        Vector2 handsDelta = secondaryHand.position - primaryHand.position;
        float distance = handsDelta.magnitude;

        attackPower = Mathf.Clamp01(distance / maxStretchDistance);

        sliderValue = Mathf.Lerp(sliderValue, attackPower, Time.deltaTime * SLIDER_LERP_SPEED);
        attackSlider.value = sliderValue;

        attackState = Mathf.FloorToInt(sliderValue * 4);
    }

    public void PickupBand()
    {
        CameraManager.Instance.ScreenShake(0.1f);
        hasBand = true;
    }

    private void AttackInitiate()
    {
        if (GameManager.Instance.BetweenRounds || !hasBand) return;

        attacking = true;
        attackPower = 0;
        sliderValue = 0;
        SoundManager.Instance.PlaySoundEffect("band_pull");
        OnAttackInitiate?.Invoke();
    }

    private void AttackHalt()
    {
        if (attackPower >= MIN_ATTACK_POWER)
        {
            FireProjectile();
            hasBand = false;
            SoundManager.Instance.PlaySoundEffect("band_release");
        }

        OnAttackHalt?.Invoke();
        ResetAttackState();
    }

    private void FireProjectile()
    {
        Vector2 fireDirection = ((Vector2)primaryHand.position - (Vector2)secondaryHand.position).normalized;

        GameObject newProjectile = Instantiate(projectile, primaryHand.position, Quaternion.identity);

        //
        newProjectile.GetComponent<RubberBand>().InitializeProjectile(attackState);

        var rb = newProjectile.GetComponent<Rigidbody2D>();
        rb.AddForce(attackPower * PROJECTILE_BASE_FORCE * fireDirection);
    }

    private void ResetAttackState()
    {
        attacking = false;
        attackPower = 0;
        sliderValue = 0;
    }

    public void SetAttackCooldown(float cooldown)
    {
        this.attackCooldown = cooldown;
    }
    public void AddAttackCooldown(float cooldown)
    {
        this.attackCooldown += cooldown;
    }
}