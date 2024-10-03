using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private float timeBetweenFrames = 0.2f;
    [SerializeField] private SpriteRenderer primaryHandSR;
    [SerializeField] private SpriteRenderer secondaryHandSR;
    private float timer;
    private int frame;
    private int frameMax = 2;
    private int animationIndex = 0;

    [SerializeField] private List<AnimationFrames> primaryHandAnimations = new();
    [SerializeField] private List<AnimationFrames> secondaryHandAnimations = new();

    
    public static PlayerAnimation Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    void Start()
    {
        PlayerAttack.OnAttackInitiate += AttackStart;
        PlayerAttack.OnAttackHalt += AttackEnd;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeBetweenFrames)
        {
            timer = 0;
            //this line will work once there are more frames, prob
            //frame = (frame + 1) % frameMax;
        }

        primaryHandSR.sprite = primaryHandAnimations[animationIndex].Sprites[frame];
        secondaryHandSR.sprite = secondaryHandAnimations[animationIndex].Sprites[frame];
    }

    void AttackStart()
    {
        animationIndex = 1;
    }

    void AttackEnd()
    {
        animationIndex = 0;
    }
}

[System.Serializable]
public struct AnimationFrames
{
    public List<Sprite> Sprites;
}