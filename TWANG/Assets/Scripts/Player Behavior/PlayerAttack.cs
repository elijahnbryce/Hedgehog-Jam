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
    [SerializeField] RubberBand projectilePrefab;
    [SerializeField] int _maxProjectiles = 1;
    [SerializeField] DragController rubberRender;
    [SerializeField] Transform primaryHand;
    [SerializeField] Transform secondaryHand;
    [SerializeField] float maxStretchDistance = 6f;

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
    }

    public void PickupBand()
    {
        CameraManager.Instance.ScreenShake(0.1f);
        
        // Min 0
        if (_shotProjectiles > 0)
            _shotProjectiles--;
    }

    public void IncreaseMaxProjectiles()
    {
        _maxProjectiles++;
    }
    public void DecreaseMaxProjectiles()
    {
        // Must have at least 1 projectile
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

        RubberBand proj = GetPooledProjectile();

        proj.transform.position = primaryHand.position + (Vector3)(fireDirection);
        proj.gameObject.SetActive(true);

        proj.InitializeProjectile(attackPower * PROJECTILE_BASE_FORCE * fireDirection);
        _shotProjectiles++;
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

    private void ResetAttackState()
    {
        attacking = false;
        attackPower = 0;
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