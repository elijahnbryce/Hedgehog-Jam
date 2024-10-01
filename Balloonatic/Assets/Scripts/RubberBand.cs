using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubberBand : MonoBehaviour
{
    private bool dead = false;
    void Start()
    {

    }

    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead) return;
        //ass code fix later
        if (collision.gameObject.tag == "Wall")
        {
            dead = true;
            StartCoroutine(nameof(DestroyProjectileCoroutine));
        }
    }

    private IEnumerator DestroyProjectileCoroutine()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
