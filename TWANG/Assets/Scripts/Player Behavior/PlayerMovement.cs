using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using DG.Tweening;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float MovementSpeed = 10f;
    [SerializeField] private float secondaryHandSpeed = 5f;
    [SerializeField] private float secondaryHandSpeedWhileAttacking = 90f;
    [SerializeField] private float followingDistance = 6f;
    [SerializeField] private AnimationCurve tensionResistanceCurve;
    [SerializeField] private float trailUpdateInterval = 0.1f;
    [SerializeField] private int maxTrailPoints = 10;
    [SerializeField] private float minDistanceForTrailPoint = 1f;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform secondHand;
    private Queue<Vector2> previousPositions = new();
    private float previousTimer = 0;

    private Rigidbody2D rigidBody;
    private Vector2 facingDirection = Vector2.right;
    private bool facingDir = true;

    private bool glued = false;
    private float glueMult; //range 0-1
    public bool FacingDir => facingDir;
    private bool Attacking => PlayerAttack.Instance.Attacking;
    private Vector2 secondCurrentPos, secondTargetPos;

    bool secondHandReadyToMove = false;
    private Vector2 mainHandPosition;
    private float footstepCounter;

    private int upgradeIndex = -1;

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
        UpdateSecondHandTarget();
        mainHandPosition = transform.position;
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

        if (upgradeIndex >= 0 && !GameUI.Instance.IsGamePaused && Input.GetKeyDown(KeyCode.Space))
        {
            UpgradeManager.Instance.ClaimUpgrade(upgradeIndex);
            upgradeIndex = -1;
        }

        UpdateTrail();
    }

    private void FixedUpdate()
    {
        if (Attacking)
        {
            Vector2 movement = GetMovementInput();
            if (secondHandReadyToMove)
                MoveSecondaryHand(movement);
            rigidBody.velocity = Vector2.zero; 
        }
        else
        {
            HandleMovement();
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
        PlayerAttack.OnAttackAim -= AttackStart;
        PlayerAttack.OnAttackHalt -= AttackEnd;
        Instance = null;
    }

    private void HandlePhysicalCollision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleEnemyCollision();
        }
        else if (collision.gameObject.CompareTag("Midair Band") || collision.gameObject.CompareTag("Landed Band"))
        {
            HandlePickupBand(collision.gameObject);
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
            case "Glue":
                if (!glued)
                    StartCoroutine(nameof(Glue));
                break;
        }
    }

    private IEnumerator Glue()
    {
        //gluemult from 1 to 0
        glueMult = 0.9f;
        glued = true;

        DOTween.To(() => glueMult, x => glueMult = x, 0f, 2f).SetEase(Ease.InExpo);

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

        PlayerAttack.Instance.PickupBand(band.GetComponent<RubberBand>().BandType);
    }

    private void UpdateTrail()
    {
        previousTimer -= Time.deltaTime;
        if (previousTimer <= 0)
        {
            previousTimer = trailUpdateInterval;

            // Only add position if we've moved enough
            if (previousPositions.Count == 0 || Vector2.Distance(mainHandPosition, transform.position) >= minDistanceForTrailPoint)
            {
                mainHandPosition = transform.position;
                previousPositions.Enqueue(mainHandPosition);

                // Maintain maximum trail length
                while (previousPositions.Count > maxTrailPoints)
                {
                    previousPositions.Dequeue();
                }
            }
        }
    }

    private void UpdateSecondHandTarget()
    {
        if (!Attacking && previousPositions.Count > 0)
        {
            Vector2 nextTarget = previousPositions.Peek();
            float distToTarget = Vector2.Distance(secondCurrentPos, nextTarget);
            float dist2 = Vector2.Distance(nextTarget, mainHandPosition);

            if (distToTarget >= minDistanceForTrailPoint && dist2 >= minDistanceForTrailPoint)
            {
                secondTargetPos = nextTarget;
                previousPositions.Dequeue();
            }
        }
    }

    private void UpdatePositions()
    {
        PlayerPosition = transform.position;

        // Update second hand position with smoother interpolation
        float speed = Attacking ? secondaryHandSpeedWhileAttacking : secondaryHandSpeed;
        secondHand.position = secondCurrentPos = Vector2.Lerp(
            secondCurrentPos,
            secondTargetPos,
            Time.deltaTime * speed
        );

        // Update target position for second hand
        if (!Attacking)
        {
            UpdateSecondHandTarget();
        }
    }

    private void HandleMovement()
    {
        if (!CanMove) return;

        Vector2 movement = GetMovementInput();

        if (movement.magnitude > 0)
        {
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

        var speedMult = CalculateSpeedMultiplier() * (glued ? 1 - glueMult : 1f);
        rigidBody.velocity = movement * MovementSpeed * speedMult;
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

    private void MoveSecondaryHand(Vector2 movement)
    {
        if (movement.magnitude > 0)
        {
            Vector2 directionToMain = (mainHandPosition - secondCurrentPos).normalized;
            Vector2 movementDirection = movement.normalized;

            float dotProduct = Vector2.Dot(movementDirection, -directionToMain);
            float tensionMultiplier = Mathf.Max(0, dotProduct);

            float distance = Vector2.Distance(mainHandPosition, secondCurrentPos);
            float tensionResistance = tensionResistanceCurve.Evaluate(distance / followingDistance);

            Vector2 newPos = (Vector2)secondHand.position +
                (movement * MovementSpeed * Time.fixedDeltaTime * (1 - (tensionResistance * tensionMultiplier)));

            Vector2 toMain = (Vector2)transform.position - newPos;
            if (toMain.magnitude > followingDistance)
            {
                newPos = (Vector2)transform.position - toMain.normalized * followingDistance;
            }

            secondTargetPos = newPos;
        }
    }

    private void AttackStart()
    {
        secondHandReadyToMove = false;
        mainHandPosition = transform.position;
        previousPositions.Clear(); 

        Vector2 direction = (secondCurrentPos - (Vector2)transform.position).normalized;
        secondTargetPos = (Vector2)transform.position + direction;
    }

    private bool IsSecondHandClose()
    {
        return Vector2.Distance(mainHandPosition, secondHand.position) < 1.5f;
    }

    private void AttackEnd()
    {
        mainHandPosition = transform.position;
        UpdateSecondHandTarget();
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
        upgradeIndex = -1;
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