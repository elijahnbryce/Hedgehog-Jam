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
        if (visualObject != null)
        {
            visualObject = transform.Find("Visual").gameObject;

            animator = visualObject.GetComponent<Animator>();
            spriteRenderers.AddRange(visualObject.GetComponents<SpriteRenderer>());

        }
        

    }

    //monobehaviour
    private void Update()
    {
        if (visualObject != null)
        {
            Vector3 visualScale = visualObject.transform.localScale;
            Debug.Log(selfEntity.physical.effectiveVelocity);
            if (selfEntity.physical.effectiveVelocity.x > 0)
            {
                visualScale.x = -Mathf.Abs(visualScale.x);
                visualObject.transform.localScale = visualScale;
            }
            else if (selfEntity.physical.effectiveVelocity.x < 0)
            {
                visualScale.x = Mathf.Abs(visualScale.x);
                visualObject.transform.localScale = visualScale;
            }
        }
        
    }

    //component-specific methods
}
