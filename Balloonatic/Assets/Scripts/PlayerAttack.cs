using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Slider attackSlider;
    private float sliderValue;
    [SerializeField] private GameObject projectile; 
    private Transform launchPoint;

    private bool attacking;
    private float attackPower;
    private float attackMax = 2.5f;
    private int attackState = 0;

    public static event Action OnAttackInitiate;
    public static event Action OnAttackHalt;


    public static PlayerAttack Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    void Start()
    {
        launchPoint = transform.GetChild(0).GetChild(0);
    }

    void Update()
    {
        sliderValue = Mathf.Clamp01(Mathf.Lerp(sliderValue, attackPower, Time.deltaTime * 2.5f));
        attackSlider.value = sliderValue;

        int flooredValue = Mathf.FloorToInt(sliderValue * 4);
        attackState = flooredValue;

        attackSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = AttackStateToColor(flooredValue);


        if (Input.GetMouseButtonDown(0))
        {
            AttackInitiate();
        }
        else if (Input.GetMouseButtonUp(0) && attacking)
        {
            AttackHalt();
        }
        //
        if (attacking)
        {
            if (attackPower < attackMax)
            {
                //attackPower += Time.deltaTime;
                attackPower = PlayerMovement.Instance.GetAttackPower();
            }
            else
                AttackHalt();
        }
    }

    void AttackInitiate()
    {
        if (GameManager.Instance.BetweenRounds)
            return;
        attacking = true;
        OnAttackInitiate?.Invoke();
    }

    void AttackHalt()
    {
        OnAttackHalt?.Invoke();
        //

        var newProjectile = Instantiate(projectile, launchPoint.position, Quaternion.identity);
        newProjectile.transform.GetChild(0).GetComponent<SpriteRenderer>().color = AttackStateToColor(attackState);
        newProjectile.GetComponent<RubberBand>().InitializeProjectile(attackState, PlayerMovement.Instance.FacingDir);

        switch (attackState)
        {
            case 1:

                break;
            case 2:

                break;
            case 3:

                break;
            default: //and 0

                break;
        }
        if(attackState!=3)
            newProjectile.GetComponent<Rigidbody2D>().AddForce(-(PlayerMovement.Instance.GetDirectionToMouse()) * 300f * attackPower);
        //
        attacking = false;
        attackPower = 0;
    }

    private Color AttackStateToColor(int state)
    {
        switch (state)
        {
            case 1:
                return Color.yellow;
            case 2:
                return Color.green;
            case 3:
                return Color.magenta;
            default: //and 0
                return Color.red;
        }
    }
}