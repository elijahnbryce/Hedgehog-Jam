using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimate : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites = new();
    [SerializeField] private float timeBetween = 0.2f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    float timer = 0;
    int ind = 0;

    void Start()
    {
        timer = timeBetween;
        if(spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = sprites[ind];
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = timeBetween;
            ind = (ind + 1) % sprites.Count;  
            spriteRenderer.sprite = sprites[ind];
        }
    }
}
