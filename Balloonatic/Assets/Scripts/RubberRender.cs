using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class DragController : MonoBehaviour
{
    private LineRenderer line;
    [SerializeField] private Transform firstHand, secondHand;
    private Transform launchPoint, releasePoint;

    private Vector3 startPos, endPos;
    private bool isDragging;


    private void Start()
    {
        PlayerAttack.OnAttackInitiate += DragStart;
        PlayerAttack.OnAttackHalt += DragEnd;

        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, Vector2.zero);
        line.SetPosition(1, Vector2.zero);
        line.enabled = false;

        launchPoint = firstHand.GetChild(0).GetChild(0);
    }

    private void DragStart()
    {
        line.enabled = true;
        isDragging = true;
        startPos = launchPoint.position;
        line.SetPosition(0, startPos);

    }

    private void Update()
    {
        if (isDragging) 
        {
            endPos = secondHand.position;
            line.SetPosition(1, endPos);
        }
    }

    private void DragEnd()
    {
        line.enabled = false;
        isDragging = false;
    }
}
 