using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPhysical : MonoBehaviour
{
    protected Entity selfEntity;

    public Rigidbody2D rb;
    public Collider2D rootCollider;

    public virtual void Initialize(Entity thisEntity)
    {
        selfEntity = thisEntity;
        rb = GetComponent<Rigidbody2D>();
        rootCollider = GetComponent<Collider2D>();
    }

    //monobehaviour
    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        selfEntity.type.OnTriggerEnter2D(collider);
    }

    //component-specific methods
    public virtual void DirectionalMove(Vector2 direction) //movement by given direction vector
    {
        rb.MovePosition(rb.position + direction.normalized * selfEntity.stats.movementSpeed * selfEntity.stats.movementSpeedMult * Time.deltaTime);
    }

    public virtual void Move(Vector2 movement)
    {
        rb.MovePosition(rb.position + movement);
    }
}
