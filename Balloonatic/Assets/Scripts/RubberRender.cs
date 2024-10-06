using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour
{
    private LineRenderer line;
    [SerializeField] private Transform firstHand, secondHand;
    [SerializeField] private ParticleSystem particleSystem; // Reference to the particle system
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

        if (particleSystem != null)
        {
            var shape = particleSystem.shape;
            shape.shapeType = ParticleSystemShapeType.SingleSidedEdge; 
            particleSystem.Stop(); 
        }
    }

    private void DragStart()
    {
        if (GameManager.Instance.BetweenRounds)
            return;
        line.enabled = true;
        isDragging = true;
        startPos = launchPoint.position;
        line.SetPosition(0, startPos);

        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            endPos = secondHand.position;
            line.SetPosition(1, endPos);

            startPos = firstHand.position;
            line.SetPosition(0, startPos);

            UpdateParticleSystem(); 
        }
    }

    private void DragEnd()
    {
        line.enabled = false;
        isDragging = false;

        if (particleSystem != null)
        {
            particleSystem.Stop();
        }
    }

    private void UpdateParticleSystem()
    {
        if (particleSystem != null)
        {
            Vector3 midpoint = (startPos + endPos) / 2f;
            particleSystem.transform.position = midpoint;

            Vector3 direction = endPos - startPos;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            particleSystem.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            var shape = particleSystem.shape;
            shape.radius = direction.magnitude / 2f; 
        }
    }
}
