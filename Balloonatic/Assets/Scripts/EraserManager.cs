using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraserManager : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites = new();
    [SerializeField] private List<Sprite> colliderSprites = new();
    public static EraserManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    void Start()
    {
        
    }

    public Sprite GetColliderSprite(int index)
    {
        return colliderSprites[index];
    }

    void Update()
    {
        
    }
}
