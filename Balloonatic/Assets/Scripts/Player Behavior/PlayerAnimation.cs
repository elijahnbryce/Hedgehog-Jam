using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
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
    private int hurtFrame = 0;

    //[SerializeField] private List<DirectionalFrames> primaryHandAnimations = new();
    [SerializeField] private List<Sprite> primaryHandSprites = new();
    [SerializeField] private List<Sprite> primaryHandGrabSprites = new();
    //[SerializeField] private List<DirectionalFrames> secondaryHandAnimations = new();
    [SerializeField] private List<Sprite> secondaryHandSprites = new();
    [SerializeField] private List<Sprite> secondaryHandGrabSprites = new();


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
            hurtFrame++;
            if (hurtFrame > 2)
                hurtFrame = 0;
            timer = 0;
            //this line will work once there are more frames, prob
            //frame = (frame + 1) % frameMax;
        }

        //primaryHandSR.sprite = primaryHandAnimations[animationIndex].Sprites[frame];

        var hurtState = 4 - GameManager.Instance.health;
        //secondaryHandSR.sprite = secondaryHandAnimations[animationIndex].Sprites[frame];

        var primaryDir = PlayerMovement.Instance.GetDirectionToMouse(true); ;
        var secondaryDir = PlayerMovement.Instance.GetDirectionToPrimaryHand();

        if (PlayerAttack.Instance.Attacking)
        {
            primaryHandSR.sprite = primaryHandGrabSprites[6 * GetDirection(primaryDir) + hurtState + (hurtState == 3 ? hurtFrame : 0)];
        }
        else
        {
            primaryHandSR.sprite = primaryHandSprites[6 * GetDirection(primaryDir) + hurtState + (hurtState == 3 ? hurtFrame : 0)];
        }

        //primaryHandSR.sprite = PlayerAttack.Instance.Attacking ? ;

        if (GameManager.Instance.BetweenRounds)
            secondaryHandSR.sprite = null;
        else
        {
            var dir2 = GetDirection(-secondaryDir);
            secondaryHandSR.sprite = PlayerAttack.Instance.Attacking ? secondaryHandGrabSprites[dir2] : secondaryHandSprites[dir2];
        }
            //secondaryHandSR.sprite = secondaryHandSprites[6 * GetDirection(secondaryDir) + hurtState + (hurtState == 3 ? hurtFrame : 0)];

    }

    //public List<int> GetDirections()
    //{
    //    Vector2 direction = PlayerMovement.Instance.GetDirectionToMouse();
    //    Vector2 direction2 = PlayerMovement.Instance.GetDirectionToPrimaryHand();
    //    var returnList = new List<int>();
    //    returnList.Append(GetDirection(direction));
    //    returnList.Append(GetDirection(direction2));
    //    return returnList;
    //}

    public int GetDirection(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (angle < 0)
            angle += 360;

        int directionIndex = Mathf.RoundToInt(angle / 45) % 8;

        string[] directionNames = { "NW", "N", "NE", "E", "SE", "S", "SW", "W" };

        int[] remappedIndices = { 7, 6, 5, 4, 3, 2, 1, 0 };
        //sorry

        int remappedIndex = remappedIndices[directionIndex];

        string directionName = directionNames[remappedIndex];

        //Debug.Log("Direction: " + directionName);

        return remappedIndex;
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
public struct DirectionalFrames
{
    public List<Sprite> Sprites;
}