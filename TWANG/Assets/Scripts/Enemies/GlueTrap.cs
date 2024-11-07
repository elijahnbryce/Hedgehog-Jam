using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueTrap : MonoBehaviour
{
    private GameObject player;
    private PlayerMovement pm;

    public Collider2D trigger;
    public float dropTime = 0.5f;

    public GameObject drop;
    public GameObject spot;

    void Start()
    {
        player = GameObject.Find("Player");
        pm = player.GetComponent<PlayerMovement>();
        StartCoroutine(GlueDropSequence());

    }

    private IEnumerator GlueDropSequence()
    {
        Vector3 dropFromPosition = drop.transform.position;
        Vector3 dropToPosition = spot.transform.position;
        for (float time = 0; time < dropTime; time += Time.deltaTime)
        {
            Vector3 currentPosition = Vector3.Lerp(dropFromPosition, dropToPosition, time / dropTime);
            drop.transform.position = currentPosition;
            yield return null;
        }
        drop.SetActive(false);
        spot.SetActive(true);
        trigger.enabled = true;

        Destroy(this.gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //this is kind of ass code, fix later!!!
        if (other.gameObject.CompareTag("Player"))
        {
            pm.MovementSpeed /= 3;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            pm.MovementSpeed *= 3;
        }
    }
}
