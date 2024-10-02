using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] public float movementSpeed;
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform secondHand;
    [SerializeField] private float followingDistance = 5f;
    private float followingSpeed = 2.5f;
    private Rigidbody2D rigidBody;
    [HideInInspector] public Vector2 PlayerPosition;
    private bool facingDir = true;
    private bool attacking = false;
    public bool FacingDir { get { return facingDir; } }
    [HideInInspector] public bool CanMove;
    [HideInInspector] public bool Moving;
    [HideInInspector] public Vector2 currentPos, targetPos;
    private Vector2 secondCurrentPos, secondTargetPos;
    private Vector2 cachedDirection;
    float counter;

    public static PlayerMovement Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    void Start()
    {
        CanMove = true;
        rigidBody = GetComponent<Rigidbody2D>();

        PlayerAttack.OnAttackInitiate += AttackStart;
        PlayerAttack.OnAttackHalt += AttackEnd;
    }

    //cleanup later
    private void AttackStart()
    {
        attacking = true;
        followingDistance = 0.5f;
        followingSpeed = 8f;
        //StartCoroutine(nameof(AttackStretch));
    }

    //cleanup later
    //private IEnumerator AttackStretch()
    //{
    //    var timer = 0f;
    //    while (timer < 2.5f)
    //    {
    //        yield return null;
    //        followingDistance += Time.deltaTime * 1.2f;
    //    }
    //}

    //cleanup later
    private void AttackEnd()
    {
        //StopCoroutine(nameof(AttackStretch));
        attacking = false;
        followingDistance = 5f;
        followingSpeed = 2.5f;
    }

    public float GetAttackPower()
    {
        return (secondHand.position - transform.position).magnitude / 5f;
    }

    void Update()
    {
        counter += Time.deltaTime;
        if (!CanMove)
        {
            transform.position = currentPos = Vector2.Lerp(currentPos, targetPos, Time.deltaTime * 5f);
        }

        //implement raycast later
        //secondTargetPos = PlayerPosition + new Vector2(facingDir ? 1 : -1, 0) * followingDistance;
        secondHand.position = secondCurrentPos = Vector3.Slerp(secondCurrentPos, secondTargetPos, Time.deltaTime * followingSpeed);
    }

    public void SnapPosition(Vector3 newPosition)
    {
        transform.position = currentPos = newPosition;
    }

    private void FixedUpdate()
    {
        PlayerPosition = transform.position;
        if (!CanMove) return;
        var movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.nearClipPlane));
        facingDir = transform.position.x > mouseWorldPosition.x;
        spriteRenderer.transform.rotation = Quaternion.Euler(new Vector3(0, facingDir ? 180 : 0, -90));

        Moving = movement.magnitude > 0;
        if (Moving)
        {
            //facingDir = movement.x < 0;

            //facingdir true : looking left
            //sprite flipx doesnt work
            if (counter > 0.15f)
            {
                //play footstep sound here
                counter = 0;
            }
        }
        if (movement.magnitude > 1) movement /= movement.magnitude;
        //i dont know how to use the event handler that was added
        //fix this later
        var evnt = GameObject.Find("EventManager").GetComponent<EventHandler>();
        var speedMult = (float)evnt.health / evnt.healthMax;
        rigidBody.velocity = movement * movementSpeed * speedMult;


        if (!attacking)
        {
            secondTargetPos = PlayerPosition + new Vector2(!facingDir ? 1 : -1, 0) * 5f;
            return;
        }

        //second hand movement
        //optimize later

        Vector3 direction = mouseWorldPosition - transform.position;
        cachedDirection = direction;


        Debug.DrawRay(transform.position, direction, Color.green);
        Debug.DrawRay(transform.position, direction * -1, Color.red);

        direction.Normalize();

        int layer = 6;
        int layerMask = 1 << layer;
        layerMask = ~layerMask;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -direction, followingDistance, layerMask);
        //Debug.Log(hit.rigidbody);
        if (hit.collider != null)
        {
            secondTargetPos = hit.point;
        }
        else
        {
            secondTargetPos = transform.position + direction * 5;
        }
    }

    public void ForceMovePlayer(Vector2 newPosition)
    {
        targetPos = newPosition;
    }
    public void TeleportPlayer(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    public Vector2 GetDirectionToMouse()
    {
        return cachedDirection;
    }
}
