using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Slider attackSlider;
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
        attackSlider.value = attackPower / attackMax;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackInitiate();
        }
        else if (Input.GetKeyUp(KeyCode.Space) && attacking)
        {
            AttackHalt();
        }
        //
        if (attacking)
        {
            if (attackPower < attackMax)
            {
                attackPower += Time.deltaTime;
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
        newProjectile.GetComponent<Rigidbody2D>().AddForce((PlayerMovement.Instance.GetDirectionToMouse()) * 300f * attackPower);
        //
        attacking = false;
        attackPower = 0;
    }
}
