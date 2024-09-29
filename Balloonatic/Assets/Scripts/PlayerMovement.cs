using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float movementSpeed;
    [Header("References")]
    private Rigidbody2D rigidBody;
    [HideInInspector] public Vector2 PlayerPosition;
    [HideInInspector] public bool CanMove;
    [HideInInspector] public bool Moving;
    [HideInInspector] public Vector2 currentPos, targetPos;
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
        if (Moving && counter > 0.15f)
        {

            //play footstep sound
            counter = 0;
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
