using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //[SerializeField] private Transform
    private Transform playerTransform;

    public static CameraManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    void Start()
    {
        playerTransform = PlayerMovement.Instance.transform;
    }

    void Update()
    {
        var newPos = Vector2.Lerp(transform.position, playerTransform.position, 5f * Time.deltaTime);
        //ass code fix later
        transform.position = new Vector3(newPos.x, newPos.y, -10);
    }
}
