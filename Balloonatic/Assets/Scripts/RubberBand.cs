using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RubberBand : MonoBehaviour
{
    private bool dead = false;
    [SerializeField] private Material whiteMat;
    private SpriteRenderer sr;
    private float lifetime = 1f;
    private int bounces = 0;
    void Start()
    {
        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        StartCoroutine(nameof(TimedDestroy));
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
            bounces--;
            if (bounces <= 0)
            {
                dead = true;
                StartCoroutine(nameof(DestroyProjectileCoroutine));
            }
        }
    }

    private IEnumerator TimedDestroy()
    {
        yield return new WaitForSeconds(lifetime);
        if (!dead)
            StartCoroutine(nameof(DestroyProjectileCoroutine));
    }

    private IEnumerator DestroyProjectileCoroutine()
    {
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponent<Collider2D>());
        //optimize later

        sr.material = whiteMat;
        sr.color = Color.white;

        var seq = DOTween.Sequence();
        seq.AppendInterval(0.1f);
        seq.Append(transform.GetChild(0).DOScale(Vector2.zero, 0.15f));
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }
}
