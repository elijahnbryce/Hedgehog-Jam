using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack Instance { get; private set; }

    public static event Action OnAttackInitiate;
    public static event Action OnAttackHalt;
    public bool Attacking => attacking;

    public int maxProjectiles => _maxProjectiles;
    public int remainingProjectiles => _shotProjectiles;

    [Header("Attack Settings")]
    [SerializeField] private LayerMask boundaryLayer;
    [SerializeField] RubberBand projectilePrefab;
    [SerializeField] int _maxProjectiles = 1;
    [SerializeField] DragController rubberRender;
    [SerializeField] Transform primaryHand;
    [SerializeField] Transform secondaryHand;
    [SerializeField] float maxStretchDistance = 6f;

    [Header("Overrides")]
    // Used for tutorial
    [Tooltip("Allows shooting even while 'game/wave' isn't running")]
    [SerializeField] bool alwaysAllowShooting = false;
    [Header("Aim Assist Settings")]
    [SerializeField] private float aimAssistAngleThreshold = 10f; // Maximum angle for aim assist in degrees
    [SerializeField] private float aimAssistStrength = 0.2f; // Visual aim assist strength (0-1)
    [SerializeField] private float aimSnapStrength = 0.8f; // Firing snap strength (0-1)

    private Entity cachedTargetEntity;
    private Vector2 originalAimDirection;
    private Vector2 assistedAimDirection;

    List<RubberBand> projectilePool = new List<RubberBand>();
    float attackPower;
    bool attacking;

    const float PROJECTILE_BASE_FORCE = 1000f;
    const float MIN_ATTACK_POWER = 0.2f;
    int _shotProjectiles;

    float attackCooldown = 0f;

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
        if (!GameManager.Instance.GameRunning && !alwaysAllowShooting)
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
        Vector2 rawAimDirection = -handsDelta.normalized;

        originalAimDirection = rawAimDirection;
        assistedAimDirection = rawAimDirection;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, rawAimDirection, 20f, boundaryLayer.value);

        if (hit.collider != null)
        {
            Entity hitEntity = hit.collider.gameObject.GetComponent<Entity>();
            if (hitEntity != null)
            {
                Vector2 directionToTarget = (hit.point - (Vector2)transform.position).normalized;
                float angle = Vector2.Angle(rawAimDirection, directionToTarget);

                if (angle <= aimAssistAngleThreshold)
                {
                    cachedTargetEntity = hitEntity;

                    assistedAimDirection = Vector2.Lerp(rawAimDirection, directionToTarget, aimAssistStrength);
                    Debug.DrawLine(transform.position, hit.point, Color.green); // Visual debug
                }
                else
                {
                    cachedTargetEntity = null;
                }
            }
        }
        else
        {
            cachedTargetEntity = null;
        }

        // Use assisted aim direction for visual feedback
        handsDelta = -assistedAimDirection * distance;
        attackPower = Mathf.Clamp01(distance / maxStretchDistance);
    }

    public void PickupBand()
    {
        CameraManager.Instance.ScreenShake(0.1f);
        
        if (_shotProjectiles > 0)
            _shotProjectiles--;
    }

    public void IncreaseMaxProjectiles()
    {
        _maxProjectiles++;
    }
    public void DecreaseMaxProjectiles()
    {
        if (_maxProjectiles > 1)
            _maxProjectiles--;
    }

    private void AttackInitiate()
    {
        if (GameManager.Instance.BetweenRounds || _shotProjectiles > maxProjectiles) return;

        attacking = true;
        attackPower = 0;
        SoundManager.Instance.PlaySoundEffect("band_pull");
        OnAttackInitiate?.Invoke();
    }

    private void AttackHalt()
    {
        if (attackPower >= MIN_ATTACK_POWER)
        {
            FireProjectile();
            SoundManager.Instance.PlaySoundEffect("band_release");
        }

        OnAttackHalt?.Invoke();
        ResetAttackState();
    }

    private void FireProjectile()
    {
        Vector2 fireDirection = ((Vector2)primaryHand.position - (Vector2)secondaryHand.position).normalized;

        if (cachedTargetEntity != null)
        {
            Vector2 directionToTarget = ((Vector2)cachedTargetEntity.transform.position - (Vector2)primaryHand.position).normalized;
            fireDirection = Vector2.Lerp(fireDirection, directionToTarget, aimSnapStrength);
        }

        RubberBand proj = GetPooledProjectile();

        proj.transform.position = primaryHand.position + (Vector3)(fireDirection);
        proj.gameObject.SetActive(true);

        proj.InitializeProjectile(attackPower * PROJECTILE_BASE_FORCE * fireDirection);
        _shotProjectiles++;
    }

    private void ResetAttackState()
    {
        attacking = false;
        attackPower = 0;
        cachedTargetEntity = null; 
    }

    RubberBand GetPooledProjectile()
    {
        for (int i = 0; i < projectilePool.Count; i++)
        {
            if (!projectilePool[i].gameObject.activeInHierarchy)
            {
                return projectilePool[i];
            }
        }
        RubberBand newPoolProj = Instantiate(projectilePrefab);
        projectilePool.Add(newPoolProj);
        return newPoolProj;
    }

    public void SetAttackCooldown(float cooldown)
    {
        attackCooldown = cooldown;
    }
    public void AddAttackCooldown(float cooldown)
    {
        attackCooldown += cooldown;
    }
}