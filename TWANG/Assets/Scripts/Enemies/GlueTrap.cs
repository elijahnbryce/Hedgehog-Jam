using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class GlueTrap : MonoBehaviour
{
    [SerializeField] private Collider2D trigger;
    [SerializeField] private GameObject drop;
    [SerializeField] private GameObject spot;
    [SerializeField] private List<Sprite> spotSprites;

    private void Start()
    {
        StartCoroutine(GlueDropSequence());
    }

    private IEnumerator GlueDropSequence()
    {
        Vector3 dropToPosition = spot.transform.position;

        drop.transform.position = dropToPosition;

        yield return new WaitForSeconds(3f);

        drop.SetActive(false);
        spot.SetActive(true);
        trigger.enabled = true;

        var spotSR = spot.GetComponent<SpriteRenderer>();

        foreach (Sprite sprite in spotSprites)
        {
            spotSR.sprite = sprite;
            yield return new WaitForSeconds(0.2f);
        }

        Destroy(gameObject);
    }
}