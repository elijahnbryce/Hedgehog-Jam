using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour
{
    [SerializeField] private Transform firstHand, secondHand;
    [SerializeField] private ParticleSystem particleSystem;
    private Transform launchPoint;
    private Vector3 startPos, endPos;
    private bool isDragging;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        PlayerAttack.OnAttackInitiate += DragStart;
        PlayerAttack.OnAttackHalt += DragEnd;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

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
        if (PlayerAttack.Instance == null)
        {
            Debug.LogError("PlayerAttack.Instance is null");
            return;
        }

        ParticleSystem ps = particleSystem;
        ParticleSystem.MainModule main = ps.main;
        var em = ps.emission;
        em.rateOverTime = strength * 10 + 2;
    }

    private void DragStart()
    {
        if (GameManager.Instance.BetweenRounds)
            return;

        spriteRenderer.enabled = true;
        isDragging = true;
        startPos = launchPoint.position;

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
            startPos = firstHand.position;
            UpdateSprite();
            UpdateParticleSystem();
        }
    }

    private void UpdateSprite()
    {
        Vector3 direction = endPos - startPos;
        float distance = direction.magnitude;

        transform.position = (startPos + endPos) / 2f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        spriteRenderer.size = new Vector2(distance, spriteRenderer.size.y);
    }

    private void DragEnd()
    {
        spriteRenderer.enabled = false;
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