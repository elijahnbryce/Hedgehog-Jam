using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class GlueTrap : MonoBehaviour
{
    [SerializeField] private Collider2D trigger;
    [SerializeField] private float dropTime = 0.5f;
    [SerializeField] private GameObject drop;
    [SerializeField] private GameObject spot;
    [SerializeField] private List<Sprite> spotSprites;

    private const float DROP_SCALE_TIME = 0.3f;
    private static readonly Vector3 DROP_SQUASH_SCALE = new Vector3(1.3f, 0.7f, 1f);
    private const float SPOT_PULSE_SCALE = 1.2f;
    private const float SPOT_PULSE_TIME = 0.5f;

    private void Start()
    {
        StartCoroutine(GlueDropSequence());
    }

    private IEnumerator GlueDropSequence()
    {
        Vector3 dropFromPosition = drop.transform.position;
        Vector3 dropToPosition = spot.transform.position;
        Vector3 originalDropScale = drop.transform.localScale;

        Sequence dropSequence = DOTween.Sequence();

        dropSequence.Append(drop.transform.DOScale(originalDropScale * 1.2f, 0.2f));

        dropSequence.Append(drop.transform.DOMove(dropToPosition, dropTime)
            .SetEase(Ease.OutBounce)
            .OnComplete(() => {
                drop.transform.DOScale(DROP_SQUASH_SCALE, DROP_SCALE_TIME)
                    .OnComplete(() => {
                        drop.transform.DOScale(originalDropScale, DROP_SCALE_TIME);
                    });
            }));

        yield return dropSequence.WaitForCompletion();

        drop.SetActive(false);
        spot.SetActive(true);
        trigger.enabled = true;

        var spotSR = spot.GetComponent<SpriteRenderer>();

        spotSR.sprite = spotSprites[0];
        yield return new WaitForSeconds(2);

        spotSR.sprite = spotSprites[1];
        yield return new WaitForSeconds(2);

        spotSR.sprite = spotSprites[2];
        yield return new WaitForSeconds(2);

        spotSR.sprite = spotSprites[3];

        Sequence fadeOutSequence = DOTween.Sequence();
        fadeOutSequence.Append(spot.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));

        yield return fadeOutSequence.WaitForCompletion();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        DOTween.Kill(drop.transform);
        DOTween.Kill(spot.transform);
    }
}