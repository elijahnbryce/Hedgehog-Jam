using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour
{
    [SerializeField] private Transform firstHand, secondHand;
    [SerializeField] private ParticleSystem particles;
    private Transform launchPoint;
    private Vector3 startPos, endPos;
    private bool isDragging;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        PlayerAttack.OnAttackAim += DragStart;
        PlayerAttack.OnAttackHalt += DragEnd;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        launchPoint = firstHand.GetChild(0).GetChild(0);

        if (particles != null)
        {
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.SingleSidedEdge;
            particles.Stop();
        }
    }

    public void UpdateBand(float strength)
    {
        if (PlayerAttack.Instance == null)
        {
            Debug.LogError("PlayerAttack.Instance is null");
            return;
        }

        ParticleSystem ps = particles;
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

        if (particles != null)
        {
            particles.Play();
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
        if (particles != null)
        {
            particles.Stop();
        }
    }

    private void UpdateParticleSystem()
    {
        if (particles != null)
        {
            Vector3 midpoint = (startPos + endPos) / 2f;
            particles.transform.position = midpoint;
            Vector3 direction = endPos - startPos;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            particles.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            var shape = particles.shape;
            shape.radius = direction.magnitude / 2f;
        }
    }
}