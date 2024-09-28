using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonMovement : MonoBehaviour
{
    [SerializeField] private Vector2 bounceBoundaries = Vector2.one * 5;
    [SerializeField] private float movementSpeed = 3f;
    private bool dirX, dirY;
    [SerializeField] private Transform stringAnchor;

    void Start()
    {

    }
    private void FixedUpdate()
    {
        transform.Translate(new Vector2(dirX ? 1 : -1, dirY ? 1 : -1) * 0.01f * movementSpeed);
        var newPos = transform.position;
        if (Mathf.Abs(newPos.x) > bounceBoundaries.x)
            dirX = !dirX;
        else if (Mathf.Abs(newPos.y) > bounceBoundaries.y)
            dirY = !dirY;

        stringAnchor.position = newPos;
    }
}
