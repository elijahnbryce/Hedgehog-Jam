using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CorpseFly : MonoBehaviour
{
    [SerializeField] private float initialSpeed = 10f;
    [SerializeField] private float launchAngle = 45f;
    [SerializeField] private float sidewaysAngle = 30f;
    private Vector3 velocity;
    private float gravity = 9.81f;

    void Start()
    {
        float verticalRadians = launchAngle * Mathf.Deg2Rad;
        float horizontalRadians = sidewaysAngle * Mathf.Deg2Rad;
        float horizontalSpeed = initialSpeed * Mathf.Cos(verticalRadians);

        //int direction = Random.Range(0, 2) * 2 - 1;

        int direction = transform.position.x > PlayerMovement.Instance.transform.position.x ? 1 : -1;
        if (direction == -1)
            GetComponent<SpriteRenderer>().flipX = true;  

        velocity = new Vector3(
            direction * horizontalSpeed * Mathf.Sin(horizontalRadians),
            initialSpeed * Mathf.Sin(verticalRadians),
            direction * horizontalSpeed * Mathf.Cos(horizontalRadians)
        );

        Destroy(gameObject, 3f);
    }

    void Update()
    {
        velocity.y -= gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
    }
}