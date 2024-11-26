using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/Pounce", fileName = "Pounce")]
public class Pounce : EntityState
{
    public float jumpDuration = 0.5f;
    public float postPounceDelay = 0.3f;
    public float maxPounceDistance = 10f; // Maximum allowed pounce distance
    public AnimationCurve jumpCurve;

    private Vector2 targetPosition;
    private Vector2 myInitialPosition;
    private Coroutine pounceSequence;

    public override void Enter()
    {
        selfEntity.physical.rb.velocity = Vector2.zero;
        myInitialPosition = selfEntity.transform.position;

        // Calculate target position with distance limitation
        Vector2 toTarget = (Vector2)selfEntity.ai.targets[0].targetGameObject.transform.position - myInitialPosition;
        float distance = toTarget.magnitude;

        if (distance > maxPounceDistance)
        {
            // Limit the pounce distance
            targetPosition = myInitialPosition + (toTarget.normalized * maxPounceDistance);
        }
        else
        {
            targetPosition = selfEntity.ai.targets[0].targetGameObject.transform.position;
        }

        if (pounceSequence == null)
        {
            pounceSequence = selfEntity.StartCoroutine(PounceSequence());
        }
    }

    public IEnumerator PounceSequence()
    {
        SoundManager.Instance.PlaySoundEffect("enemy_attack");
        Vector2 shadowPosition = selfEntity.visual.shadowObject.transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            // Use Time.fixedDeltaTime for physics-based movement
            elapsedTime += Time.fixedDeltaTime;
            float normalizedTime = elapsedTime / jumpDuration;

            // Calculate positions using SmoothStep for smoother acceleration/deceleration
            float t = Mathf.SmoothStep(0f, 1f, normalizedTime);
            Vector2 currentPosition = selfEntity.transform.position;
            float jumpHeight = jumpCurve.Evaluate(normalizedTime);

            // Calculate next position
            Vector2 nextPosition = Vector2.Lerp(myInitialPosition, targetPosition, t);
            nextPosition.y += jumpHeight;

            // Update shadow
            Vector2 shadowLerp = Vector2.Lerp(shadowPosition, shadowPosition, t);
            shadowLerp.y -= jumpHeight;

            // Use MovePosition for physics-based movement
            selfEntity.physical.rb.MovePosition(nextPosition);
            selfEntity.visual.shadowObject.transform.localPosition = shadowLerp;

            yield return new WaitForFixedUpdate();
        }

        // Ensure we reach exactly the target position
        selfEntity.physical.rb.MovePosition(targetPosition);

        yield return new WaitForSeconds(postPounceDelay);
        pounceSequence = null;
        ManualExit();
    }
}