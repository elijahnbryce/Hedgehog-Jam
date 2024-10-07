using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour
{
    private LineRenderer line;
    [SerializeField] private Transform firstHand, secondHand;
    [SerializeField] private ParticleSystem particleSystem;
    private Transform launchPoint, releasePoint;

    private Vector3 startPos, endPos;
    private bool isDragging;

    private Color currentColor, targetColor;

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

    public void UpdateBand(float strength)
    {
        // Ensure PlayerAttack.Instance is not null
        if (PlayerAttack.Instance == null)
        {
            Debug.LogError("PlayerAttack.Instance is null");
            return;
        }

        targetColor = PlayerAttack.Instance.CurrentColor;
        //targetColor = PlayerAttack.Instance.AttackStateToColor((int)(strength * 4));
        //strength 0-1
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * 10f);

        // Update line renderer if it exists
        if (line != null)
        {
            line.startColor = currentColor;
            line.endColor = currentColor;
        }
        else
        {
            Debug.LogWarning("Line renderer is null");
        }

        // Get ParticleSystem component
        ParticleSystem ps = particleSystem;
        if (ps != null)
        {
            ParticleSystem.MainModule main = ps.main;
            main.startColor = new ParticleSystem.MinMaxGradient(currentColor);
            main.startSpeed = strength * 0.5f + 0.25f;

            var em = ps.emission;
            em.rateOverTime = strength * 10 + 2;
        }
        else
        {
            Debug.LogError("ParticleSystem component not found on this GameObject");
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
