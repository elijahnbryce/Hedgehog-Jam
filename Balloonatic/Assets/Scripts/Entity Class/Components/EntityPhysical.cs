using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPhysical : MonoBehaviour
{
    protected Entity selfEntity;

    public Rigidbody2D rb;
    public Collider2D rootCollider;
    public List<Collider2D> colliderInfo = new List<Collider2D>();

    private Vector2 previousPosition = Vector2.zero;
    private Vector2 currentPosition = Vector2.zero;
    [HideInInspector] public Vector2 effectiveVelocity = Vector2.zero;

    public virtual void Initialize(Entity thisEntity)
    {
        selfEntity = thisEntity;
        rb = GetComponent<Rigidbody2D>();
        rootCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        previousPosition = transform.position;
        currentPosition = transform.position;
    }

    private void Update()
    {
        currentPosition = transform.position;
        effectiveVelocity = currentPosition - previousPosition;
        previousPosition = currentPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    { 
	colliderInfo.Add(collider); 
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
	colliderInfo.Remove(collider);
    }

    //component-specific methods
    public virtual void DirectionalMove(Vector2 direction) //movement by given direction vector
    {
        rb.velocity += direction.normalized * selfEntity.stats.effectiveMovementSpeed;
    }

    public virtual void Move(Vector2 movement)
    {
        rb.MovePosition(rb.position + movement);
    }
 
    public virtual void ClampToSpeed()
    {
	rb.velocity = Vector2.ClampMagnitude(rb.velocity, selfEntity.stats.effectiveMovementSpeed);
    }
}
