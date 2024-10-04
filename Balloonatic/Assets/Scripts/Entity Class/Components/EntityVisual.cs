using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityVisual : MonoBehaviour
{
    protected Entity selfEntity;
    //contains visual information about the entity

    public GameObject visualObject;
    public Animator animator;
    public List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();


    public virtual void Initialize(Entity thisEntity)
    {
        selfEntity = thisEntity;
        visualObject = transform.Find("Visual").gameObject;

        animator = visualObject.GetComponent<Animator>();
        spriteRenderers.AddRange(visualObject.GetComponents<SpriteRenderer>());

    }

    //monobehaviour
    private void Update()
    {
        
    }

    //component-specific methods
}
