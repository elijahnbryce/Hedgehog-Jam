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

    //component-specific methods
    public virtual void DirectionalMove(Vector2 direction) //movement by given direction vector
    {
        rb.MovePosition(rb.position + direction.normalized * selfEntity.stats.movementSpeed * selfEntity.stats.movementSpeedMult * Time.deltaTime);
    }
}
