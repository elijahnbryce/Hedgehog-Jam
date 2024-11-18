using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityVisual : MonoBehaviour
{
    protected Entity selfEntity;
    public GameObject visualObject;
    public GameObject shadowObject;
    public Animator animator;
    public List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

    [SerializeField] private List<Sprite> directionalSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> altDirectionalSprites = new List<Sprite>();

    private SpriteRenderer mainRenderer;

    private float timer;
    private bool alternate;

    public virtual void Initialize(Entity thisEntity)
    {
        visualObject = transform.Find("Visual")?.gameObject;
        shadowObject = transform.Find("DropShadow")?.gameObject;
        selfEntity = thisEntity;
        if (visualObject != null)
        {
            animator = visualObject.GetComponent<Animator>();
            spriteRenderers.AddRange(visualObject.GetComponents<SpriteRenderer>());
            mainRenderer = visualObject.GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (visualObject != null)
        {
            if (directionalSprites.Count >= 8 && mainRenderer != null)
            {
                timer += Time.deltaTime;
                if(timer > 0.2f)
                {
                    timer = 0;
                    alternate = !alternate;
                }
                
                UpdateDirectionalSprite();
            }
            else
            {
                UpdateSimpleFlip();
            }
        }
    }

    private void UpdateSimpleFlip()
    {
        Vector3 visualScale = visualObject.transform.localScale;
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

    private void UpdateDirectionalSprite()
    {
        Vector2 velocity = selfEntity.physical.effectiveVelocity;
        if (velocity.magnitude < 0.01f) return;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;

        // Normalize angle to 0-360
        if (angle < 0) angle += 360f;

        // Map angles to sprite indices
        // 0: left (180°)
        // 1: up left (135°)
        // 2: up (90°)
        // 3: up right (45°)
        // 4: right (0°)
        // 5: down right (315°)
        // 6: down (270°)
        // 7: down left (225°)

        int spriteIndex;
        if (angle >= 337.5f || angle < 22.5f)  // right
            spriteIndex = 4;
        else if (angle >= 22.5f && angle < 67.5f)  // up right
            spriteIndex = 3;
        else if (angle >= 67.5f && angle < 112.5f)  // up
            spriteIndex = 2;
        else if (angle >= 112.5f && angle < 157.5f)  // up left
            spriteIndex = 1;
        else if (angle >= 157.5f && angle < 202.5f)  // left
            spriteIndex = 0;
        else if (angle >= 202.5f && angle < 247.5f)  // down left
            spriteIndex = 7;
        else if (angle >= 247.5f && angle < 292.5f)  // down
            spriteIndex = 6;
        else  // down right
            spriteIndex = 5;

        mainRenderer.sprite = alternate ? directionalSprites[spriteIndex] : altDirectionalSprites[spriteIndex];
    }
}