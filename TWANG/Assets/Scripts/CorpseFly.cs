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
        // Convert angles to radians
        float verticalRadians = launchAngle * Mathf.Deg2Rad;
        float horizontalRadians = sidewaysAngle * Mathf.Deg2Rad;

        // Calculate initial velocity components
        float horizontalSpeed = initialSpeed * Mathf.Cos(verticalRadians);
        velocity = new Vector3(
            horizontalSpeed * Mathf.Sin(horizontalRadians), // X component
            initialSpeed * Mathf.Sin(verticalRadians),      // Y component
            horizontalSpeed * Mathf.Cos(horizontalRadians)  // Z component
        );

        Destroy(gameObject, 3f);
    }

    void Update()
    {
        // Apply gravity to Y velocity
        velocity.y -= gravity * Time.deltaTime;

        // Update position
        transform.position += velocity * Time.deltaTime;
    }
}