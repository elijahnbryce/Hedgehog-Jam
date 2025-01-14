using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float MovementSpeed = 10f;
    [SerializeField] private float secondaryHandSpeed = 5f; // Controls how fast secondary hand moves
    [SerializeField] private float secondaryHandSpeedWhileAttacking = 90f;
    [SerializeField] private float followingDistance = 6f;
    [SerializeField] private AnimationCurve tensionResistanceCurve;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform secondHand;

    private Rigidbody2D rigidBody;
    private Vector2 facingDirection = Vector2.right; // Start facing right
    private bool facingDir = true;

    private bool glued = false;
    public bool FacingDir => facingDir;
    private bool Attacking => PlayerAttack.Instance.Attacking;
    private Vector2 secondCurrentPos, secondTargetPos;

    bool secondHandReadyToMove = false;
    private Vector2 mainHandPosition;
    private float footstepCounter;

    private int upgradeIndex = -1; // Default to invalid index when no upgrade is selected

    private const float FOOTSTEP_INTERVAL = 0.75f;
    private const float POSITION_LERP_SPEED = 5f;

    [HideInInspector] public bool CanMove = true;
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

        // Initialize second hand position
        UpdateSecondHandTarget(Vector2.right);
    }

    private void RegisterEventHandlers()
    {
        PlayerAttack.OnAttackAim += AttackStart;
        PlayerAttack.OnAttackHalt += AttackEnd;
    }

    private void Update()
    {
        footstepCounter += Time.deltaTime;
        UpdatePositions();

        if (!secondHandReadyToMove)
            secondHandReadyToMove = IsSecondHandClose();

        // Check for upgrade selection
        if (upgradeIndex >= 0 && !GameUI.Instance.IsGamePaused && Input.GetKeyDown(KeyCode.Space))
        {
            UpgradeManager.Instance.ClaimUpgrade(upgradeIndex);
            upgradeIndex = -1; // Reset after claiming
        }
    }

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

    private void OnDestroy()
    {
        Instance = null;
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
            case "Landed Band":
                HandlePickupBand(collision.gameObject);
                break;
            case "Glue":
                if (!glued)
                    StartCoroutine(nameof(Glue));
                break;
        }
    }

    private IEnumerator Glue()
    {
        glued = true;
        yield return new WaitForSeconds(2f);
        glued = false;
    }

    private void HandlePickupBand(GameObject band)
    {
        if (PlayerAttack.Instance.HoldingBand)
        {
            return;
        }

        Destroy(band.GetComponent<Collider2D>());

        var seq = DOTween.Sequence();
        band.transform.DOMove(band.transform.position + Vector3.up, 0.2f);
        band.transform.DORotate(new Vector3(0, 0, 180), 0.2f);
        seq.Append(band.transform.DOScale(Vector2.zero, 0.2f)).SetEase(Ease.InQuad);
        seq.AppendCallback(() => Destroy(band));

        PlayerAttack.Instance.PickupBand();
    }

    private void FixedUpdate()
    {
        if (Attacking)
        {
            // When attacking, move secondary hand with WASD
            Vector2 movement = GetMovementInput();
            if (secondHandReadyToMove)
                MoveSecondaryHand(movement);
            rigidBody.velocity = Vector2.zero; // Keep primary hand still
        }
        else
        {
            // Normal movement
            HandleMovement();
        }
    }

    private void UpdatePositions()
    {
        PlayerPosition = transform.position;

        // Smoothly move secondary hand to target
        secondHand.position = secondCurrentPos = Vector3.Slerp(
            secondCurrentPos,
            secondTargetPos,
            Time.deltaTime * (Attacking ? secondaryHandSpeedWhileAttacking : secondaryHandSpeed)
        );
    }

    private void MoveSecondaryHand(Vector2 movement)
    {
        if (movement.magnitude > 0)
        {
            Vector2 directionToMain = (mainHandPosition - secondCurrentPos).normalized;
            Vector2 movementDirection = movement.normalized;

            float dotProduct = Vector2.Dot(movementDirection, -directionToMain);
            float tensionMultiplier = Mathf.Max(0, dotProduct);

            float tensionResistance = tensionResistanceCurve.Evaluate(Vector2.Distance(mainHandPosition, secondCurrentPos) / followingDistance);

            Vector2 newPos = (Vector2)secondHand.position + (movement * MovementSpeed * Time.fixedDeltaTime * (1 - (tensionResistance * tensionMultiplier)));

            Vector2 toMain = (Vector2)transform.position - newPos;
            if (toMain.magnitude > followingDistance)
            {
                newPos = (Vector2)transform.position - toMain.normalized * followingDistance;
            }

            secondTargetPos = newPos;
        }
    }

    private void HandleMovement()
    {
        if (!CanMove) return;

        Vector2 movement = GetMovementInput();

        if (movement.magnitude > 0)
        {
            // Update facing direction when moving
            facingDirection = movement.normalized;
            Moving = true;

            if (footstepCounter > FOOTSTEP_INTERVAL)
            {
                SoundManager.Instance.PlaySoundEffect("player_walk");
                footstepCounter = 0;
            }
        }
        else
        {
            Moving = false;
        }

        // Apply movement
        var speedMult = CalculateSpeedMultiplier() * (glued ? 0.5f : 1f);
        rigidBody.velocity = movement * MovementSpeed * speedMult;

        // Update second hand target based on raycast
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

    private float CalculateSpeedMultiplier()
    {
        var gm = GameManager.Instance;
        var healthRatio = Mathf.Clamp01(gm.GetHealthRatio() * 2);
        return healthRatio * gm.GetPowerMult(UpgradeType.Lightning, 1.15f);
    }

    private void UpdateSecondHandTarget(Vector2 movement)
    {
        Vector2 rayDirection;

        if (movement.magnitude > 0)
        {
            rayDirection = -movement.normalized; // Opposite of movement
        }
        else
        {
            rayDirection = -facingDirection; // Use stored facing direction
        }

        int layerMask = ~((1 << 6) | (1 << 12));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, followingDistance, layerMask);

        if (hit.collider != null)
        {
            secondTargetPos = hit.point;
        }
        else
        {
            secondTargetPos = (Vector2)transform.position + (rayDirection * followingDistance);
        }
    }

    private void AttackStart()
    {
        secondHandReadyToMove = false;
        mainHandPosition = transform.position;

        Vector2 direction = (secondCurrentPos - (Vector2)transform.position).normalized;
        secondTargetPos = (Vector2)transform.position + direction;
    }

    private bool IsSecondHandClose()
    {
        return Vector2.Distance(mainHandPosition, secondHand.position) < 1.5f;
    }

    private void AttackEnd()
    {
        // Reset second hand position
        UpdateSecondHandTarget(facingDirection);
    }

    public Vector2 GetDirectionOfPrimaryHand()
    {
        if (Attacking)
            return ((Vector2)secondHand.position - mainHandPosition).normalized;
        return -facingDirection;
    }

    public Vector2 GetDirectionToPrimaryHand()
    {
        var dir = (Vector2)secondHand.position - (Vector2)transform.position;
        return dir.normalized;
    }

    private void CancelUpgradeSelection()
    {
        upgradeIndex = -1; // Reset upgrade selection when leaving trigger
    }

    private void HandleUpgradeSelection(Collider2D collision)
    {
        upgradeIndex = int.Parse(collision.gameObject.name);
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
}