using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class RubberBand : MonoBehaviour
{
    [SerializeField] private List<Sprite> rubberBandSprites = new();
    [SerializeField] private GameObject landedPrefab;

    private bool dead = false;
    [SerializeField] private Material whiteMat;
    private Material defaultMat;
    private SpriteRenderer sr;
    private float lifetime = 3f;
    private int bounces = 0;

    public float initialSpeed = 5f;
    public float spiralGrowthRate = 0.5f;
    public float rotationSpeed = 100f;
    public float attackPower = 1f;

    private int attackState;

    private Rigidbody2D rb;
    void Start()
    {
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PlayerEffect"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("LevelBoundary"), LayerMask.NameToLayer("PlayerEffect"));

        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        defaultMat = sr.material;
        StartCoroutine(nameof(TimedDestroy));

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!dead)
        {
            UpdateRubberBandSprite();
            //Debug.Log(rb.velocity.magnitude);
            //if (rb.velocity.magnitude < 2f)
            //{
            //    StartCoroutine(nameof(DestroyProjectileCoroutine));
            //}
        }
    }

    void UpdateRubberBandSprite()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;

        // Normalize angle to positive
        if (angle < 0) angle += 360f;

        if (angle > 180f)
            angle -= 180f;

        int spriteIndex;

        if (angle <= 15f || angle >= 165f)  // Increased tolerance for 0/180
            spriteIndex = 4;
        else if (angle > 15f && angle < 37.5f)  // 30°
            spriteIndex = 5;
        else if (angle >= 37.5f && angle < 52.5f)  // 45°
            spriteIndex = 6;
        else if (angle >= 52.5f && angle < 75f)  // 60°
            spriteIndex = 7;
        else if (angle >= 75f && angle < 105f)  // 90°
            spriteIndex = 0;
        else if (angle >= 105f && angle < 127.5f)  // 120°
            spriteIndex = 1;
        else if (angle >= 127.5f && angle < 142.5f)  // 135°
            spriteIndex = 2;
        else if (angle >= 142.5f && angle < 165f)  // 150°
            spriteIndex = 3;
        else
            spriteIndex = 4;  // Default to horizontal sprite
        // Update the sprite
        if (rubberBandSprites.Count > spriteIndex)
        {
            sr.sprite = rubberBandSprites[spriteIndex];
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead) return;

        // Player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Band hit player");
            dead = true;
            Destroy(gameObject);
            PlayerAttack.Instance.PickupBand();
        }

        //ass code fix later
        if (collision.gameObject.tag == "Wall")
        {
            SoundManager.Instance.PlaySoundEffect("band_hit");
            CameraManager.Instance.ScreenShake();
            if (bounces <= 0)
            {
                dead = true;
                StartCoroutine(nameof(DestroyProjectileCoroutine));
            }
            else
            {
                bounces--;

                StartCoroutine(nameof(FlashWhite));
            }
        }
        // idk enemy tag
        if (collision.gameObject.tag == "Enemy")
        {
            collision.transform.GetComponent<Entity>().stats.TakeDamage((int)attackPower);


            //ass code fix later

            if (!GameManager.Instance.upgradeList.ContainsKey(UpgradeType.Ghost))
            {
                dead = true;
                StartCoroutine(nameof(DestroyProjectileCoroutine));
            }
            else { GameManager.Instance.DecPowerUp(UpgradeType.Ghost); }
        }
    }

    private bool SeekEnemies()
    {
        float radius = 5f;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius);

        bool found = false;
        foreach (Collider2D collider in hitColliders)
        {
            if (!found && collider.CompareTag("Enemy"))
            {
                found = true;
                var prevVelocity = rb.velocity.magnitude;
                rb.velocity = Vector2.zero; 
                rb.velocity = (collider.transform.position - transform.position).normalized * prevVelocity;  
            }
        }

        return found;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Level Boundary")
        {
            StartCoroutine(nameof(DestroyProjectileCoroutine));
        }
    }

    public void InitializeProjectile(int state)
    {
        attackState = state;
        switch (state)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
        bounces = 1;
    }

    private IEnumerator FlashWhite()
    {
        sr.material = whiteMat;
        yield return new WaitForSeconds(0.1f);
        sr.material = defaultMat;
    }

    private IEnumerator TimedDestroy()
    {
        yield return new WaitForSeconds(lifetime);
        if (!dead)
            StartCoroutine(nameof(DestroyProjectileCoroutine));
    }

    private IEnumerator DestroyProjectileCoroutine()
    {
        dead = true;

        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponent<Collider2D>());
        //optimize later

        sr.material = whiteMat;
        var seq = DOTween.Sequence();
        seq.AppendInterval(0.1f);
        seq.Append(transform.GetChild(0).DOScale(Vector2.zero, 0.15f));
        yield return new WaitForSeconds(0.25f);

        var newBand = Instantiate(landedPrefab, transform.position, Quaternion.identity).transform;
        //newBand.Rotate(0, 0, Random.Range(0, 3) * 90);
        var sr2 = newBand.GetComponent<SpriteRenderer>();
        sr2.material = whiteMat;
        var seq2 = DOTween.Sequence();
        seq2.Append(newBand.transform.DOPunchScale(Vector3.one * .25f, 0.15f));
        seq2.AppendCallback(() =>
        {
            sr2.material = defaultMat;
        });
        Destroy(gameObject, 0.5f);
    }
}
