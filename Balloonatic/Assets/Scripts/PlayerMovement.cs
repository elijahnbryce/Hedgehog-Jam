using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float movementSpeed;
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform secondHand;
    private Rigidbody2D rigidBody;
    [HideInInspector] public Vector2 PlayerPosition;
    private bool facingDir = true;
    [HideInInspector] public bool CanMove;
    [HideInInspector] public bool Moving;
    [HideInInspector] public Vector2 currentPos, targetPos;
    private Vector2 secondCurrentPos, secondTargetPos;
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
    }

    void Update()
    {
        counter += Time.deltaTime;
        if (!CanMove)
        {
            transform.position = currentPos = Vector2.Lerp(currentPos, targetPos, Time.deltaTime * 5f);
        }

        //implement raycast later
        secondTargetPos = PlayerPosition + new Vector2(facingDir ? 1 : -1, 0) * 5f;
        secondHand.position = secondCurrentPos = Vector3.Slerp(secondCurrentPos, secondTargetPos, Time.deltaTime * 2.5f);
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


        Moving = movement.magnitude > 0;
        if (Moving)
        {
            facingDir = movement.x < 0;
            //facingdir true : looking left
            spriteRenderer.transform.rotation = Quaternion.Euler(new Vector3(0, facingDir ? 0 : 180, -90));
            //sprite flipx doesnt work
            if (counter > 0.15f)
            {
                //play footstep sound here
                counter = 0;
            }
        }
        if (movement.magnitude > 1) movement /= movement.magnitude;
        rigidBody.velocity = movement * movementSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //switch (collision.gameObject.tag)
        //{

        //}
    }

    public void ForceMovePlayer(Vector2 newPosition)
    {
        targetPos = newPosition;
    }
    public void TeleportPlayer(Vector2 newPosition)
    {
        transform.position = newPosition;
    }
}
