using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float MovementSpeed;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform secondHand;
    [SerializeField] private float followingDistance = 6f;

    private float followingSpeed = 100f;
    private Rigidbody2D rigidBody;
    private bool facingDir = true;
    private bool attacking = false;
    private Vector2 secondCurrentPos, secondTargetPos;
    private Vector2 mainHandPosition;
    private Vector2 cachedMovementDirection;
    private Vector2 cachedDirection;
    private float footstepCounter;

    // upgrade related fields
    private float upgradeTimer = 8f;
    private int upgradeIndex = 0;

    private const float FOOTSTEP_INTERVAL = 0.75f;
    private const float POSITION_LERP_SPEED = 5f;
    private const float ATTACK_FOLLOWING_SPEED = 8f;

    public bool FacingDir => facingDir;
    [HideInInspector] public bool CanMove;
    [HideInInspector] public bool Moving;
    [HideInInspector] public Vector2 PlayerPosition;
    [HideInInspector] public Vector2 currentPos, targetPos;
    public static PlayerMovement Instance { get; private set; }

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
        CanMove = true;
        rigidBody = GetComponent<Rigidbody2D>();
        RegisterEventHandlers();
    }

    private void Update()
    {
        UpdatePositions();
        if (attacking)
        {
            HandleSecondHandAttackControl();
        }
    }

    private void HandleSecondHandAttackControl()
    {
        // Get mouse position in world space
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        secondTargetPos = mousePos;
    }

    private void FixedUpdate()
    {
        HandleMovement();
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

    private void InitializeComponents()
    {
        CanMove = true;
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void RegisterEventHandlers()
    {
        PlayerAttack.OnAttackInitiate += AttackStart;
        PlayerAttack.OnAttackHalt += AttackEnd;
    }

    // update methods
    private void UpdatePositions()
    {
        footstepCounter += Time.deltaTime;
        UpdatePlayerPosition();
        UpdateSecondHandPosition();
    }

    private void UpdatePlayerPosition()
    {
        if (!CanMove)
        {
            transform.position = currentPos = Vector2.Lerp(currentPos, targetPos, Time.deltaTime * POSITION_LERP_SPEED);
        }
        PlayerPosition = transform.position;
    }

    private void UpdateSecondHandPosition()
    {
        if (attacking)
        {
            // Smooth follow of mouse during attack
            secondHand.position = secondCurrentPos = Vector3.Lerp(
                secondCurrentPos,
                secondTargetPos,
                Time.deltaTime * ATTACK_FOLLOWING_SPEED
            );
        }
        else
        {
            // Normal following behavior
            secondHand.position = secondCurrentPos = Vector3.Lerp(
                secondCurrentPos,
                secondTargetPos,
                Time.deltaTime * followingSpeed
            );
        }
    }

    // movement handling methods
    private void HandleMovement()
    {
        if (!CanMove || attacking) return;

        Vector2 movement = GetMovementInput();
        UpdateFacingDirection(movement);
        HandleMovingState(movement);
        ApplyMovement(movement);
        UpdateSecondHandTarget(movement);
    }

    private Vector2 GetMovementInput()
    {
        var movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (GameManager.Instance.upgradeList.ContainsKey(UpgradeType.Confusion))
        {
            movement *= -Vector2.one;
        }

        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        return movement;
    }

    private void UpdateFacingDirection(Vector2 movement)
    {
        if (movement.magnitude > 0)
        {
            cachedMovementDirection = movement;
        }
    }

    private void HandleMovingState(Vector2 movement)
    {
        Moving = movement.magnitude > 0;
        if (Moving && footstepCounter > FOOTSTEP_INTERVAL)
        {
            SoundManager.Instance.PlaySoundEffect("player_walk");
            footstepCounter = 0;
        }
    }

    private void ApplyMovement(Vector2 movement)
    {
        var speedMult = CalculateSpeedMultiplier();
        rigidBody.velocity = movement * MovementSpeed * speedMult;
    }

    private float CalculateSpeedMultiplier()
    {
        var gm = GameManager.Instance;
        var healthRatio = Mathf.Clamp01(gm.GetHealthRatio() * 2);
        return healthRatio * gm.GetPowerMult(UpgradeType.Lightning, 1.5f);
    }

    private void UpdateSecondHandTarget(Vector2 movement)
    {
        // Cast ray in opposite direction of movement
        Vector2 rayDirection = -cachedMovementDirection;
        int layerMask = ~((1 << 6) | (1 << 12)); // Using existing layer mask

        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, followingDistance, layerMask);

        if (hit.collider != null)
        {
            // If we hit something, place the second hand at the hit point
            secondTargetPos = hit.point;
        }
        else
        {
            // If we didn't hit anything, place it at the maximum distance
            secondTargetPos = (Vector2)transform.position + (rayDirection * followingDistance);
        }
    }

    private void UpdateNormalSecondHandTarget()
    {
        secondTargetPos = PlayerPosition + new Vector2(!facingDir ? 1 : -1, 0) * 5f;
    }

    private void UpdateAttackingSecondHandTarget()
    {
        //Vector3 direction = CalculateMouseDirection();

        Vector3 direction = GetDirectionOfPrimaryHand();

        cachedDirection = direction;
        direction.Normalize();

        HandleSecondHandRaycast(direction);
    }

    // attack related methods
    private void AttackStart()
    {
        attacking = true;
        rigidBody.velocity = Vector2.zero; // Stop movement
        mainHandPosition = transform.position; // Store main hand position
    }

    private void AttackEnd()
    {
        attacking = false;
    }

    public float GetAttackPower()
    {
        return (secondHand.position - transform.position).magnitude / followingDistance;
    }

    // utility methods
    private void PlayFootstepSound()
    {
        SoundManager.Instance.PlaySoundEffect("player_walk");
    }

    //private Vector3 GetMouseWorldPosition()
    //{
    //    Vector3 mouseScreenPosition = Input.mousePosition;
    //    return Camera.main.ScreenToWorldPoint(new Vector3(
    //        mouseScreenPosition.x,
    //        mouseScreenPosition.y,
    //        Camera.main.nearClipPlane
    //    ));
    //}

    //private Vector3 CalculateMouseDirection()
    //{
    //    return GetMouseWorldPosition() - transform.position;
    //}

    private void HandleSecondHandRaycast(Vector3 direction)
    {
        int layerMask = ~((1 << 6) | (1 << 12));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -direction, followingDistance, layerMask);

        secondTargetPos = hit.collider != null && hit.collider.CompareTag("Player")
            ? hit.point
            : (Vector2)(transform.position + direction * 5);
    }

    // public utility methods
    public void SnapPosition(Vector3 newPosition)
    {
        transform.position = currentPos = newPosition;
    }



    //public Vector2 GetDirectionToMouse()
    //{
    //    return cachedDirection;
    //}

    //public Vector2 GetDirectionToMouse(bool _)
    //{
    //    if (!PlayerAttack.Instance.Attacking)
    //    {
    //        if (Moving)
    //        {
    //            cachedMovementDirection = -rigidBody.velocity;
    //        }
    //        return cachedMovementDirection;
    //    }
    //    return cachedDirection;
    //}

    public Vector2 GetDirectionOfPrimaryHand()
    {
        return cachedMovementDirection;
    }

    public Vector2 GetDirectionToPrimaryHand()
    {
        var dir = secondHand.position - transform.position;
        dir.Normalize();
        return dir;
    }

    // collision handling methods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleTriggerCollision(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandlePhysicalCollision(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        HandleTriggerExit(collision);
    }

    private void HandleTriggerCollision(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Selection":
                HandleUpgradeSelection(collision);
                break;
            case "Enemy":
                HandleEnemyCollision();
                break;
            case "Coin":
                HandleCoinCollection(collision);
                break;
        }
    }

    private void HandlePhysicalCollision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleEnemyCollision();
        }
    }

    private void HandleTriggerExit(Collider2D collision)
    {
        if (collision.CompareTag("Selection"))
        {
            CancelUpgradeSelection();
        }
    }

    // upgrade handling methods
    private void HandleUpgradeSelection(Collider2D collision)
    {
        upgradeIndex = int.Parse(collision.gameObject.name);
        StartCoroutine(nameof(UpgradeCountdown));
    }

    private void CancelUpgradeSelection()
    {
        StopCoroutine(nameof(UpgradeCountdown));
        upgradeTimer = 8f;
    }

    private void HandleEnemyCollision()
    {
        Debug.Log("Player damaged");
        GameManager.Instance.UpdateHealth();
    }

    private void HandleCoinCollection(Collider2D collision)
    {
        collision.GetComponent<Coin>().ClaimCoin();
        Debug.Log("Picked up coin.");
    }

    private IEnumerator UpgradeCountdown()
    {
        while (upgradeTimer > 0)
        {
            upgradeTimer -= Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                upgradeTimer = 0;
            }
            yield return null;
        }
        UpgradeManager.Instance.ClaimUpgrade(upgradeIndex);
    }
}