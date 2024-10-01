using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Slider attackSlider;
    private float sliderValue;
    [SerializeField] private GameObject projectilePrefab;
    private Transform launchPoint;

    private bool attacking;
    private float attackPower;
    private float attackMax = 2.5f;

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
        sliderValue = Mathf.Lerp(sliderValue, attackPower, Time.deltaTime * 2.5f);
        attackSlider.value = Mathf.Clamp01(sliderValue);

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
        attacking = true;
        OnAttackInitiate?.Invoke();
    }

    void AttackHalt()
    {
        OnAttackHalt?.Invoke();
        //

        var newProjectile = Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);
        newProjectile.GetComponent<Rigidbody2D>().AddForce(-(PlayerMovement.Instance.GetDirectionToMouse()) * 300f * attackPower);
        //
        attacking = false;
        attackPower = 0;
    }
}
