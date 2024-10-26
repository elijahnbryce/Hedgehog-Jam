using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RubberBand : MonoBehaviour
{
    [SerializeField] private List<Sprite> rubberBandSprites = new();
    [SerializeField] private GameObject landedPrefab;

    private bool dead = false;
    [SerializeField] private Material whiteMat;
    private SpriteRenderer sr;
    private float lifetime = 1f;
    private int bounces = 0;
    private bool spiral = false;
    private bool facingDir = false;

    //this code sucks, cleanup later
    public float initialSpeed = 5f;
    public float spiralGrowthRate = 0.5f;
    public float rotationSpeed = 100f;
    public float attackPower = 1f;
    private float currentAngle = 0f;
    private float currentRadius = 0f;

    private int attackState;

    private Rigidbody2D rb;
    void Start()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PlayerEffect"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("LevelBoundary"), LayerMask.NameToLayer("PlayerEffect"));

        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        StartCoroutine(nameof(TimedDestroy));

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(!dead)
            UpdateRubberBandSprite();
    }

    void UpdateRubberBandSprite()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;

        // Normalize angle to positive
        if (angle < 0) angle += 360f;

        if (angle > 180f)
            angle -= 180f;

        int spriteIndex;

        if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
            spriteIndex = 4;
        else if (angle > 0f && angle < 37.5f)  // 30° with some tolerance
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
        else  // 150°
            spriteIndex = 3;
        // Update the sprite
        if (rubberBandSprites.Count > spriteIndex)
        {
            sr.sprite = rubberBandSprites[spriteIndex];
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead) return;

        if (collision.gameObject.layer == 6)
        {
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
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
            if (!GameManager.Instance.upgradeList.ContainsKey(UpgradeType.Ghost))
            {
                dead = true;
                StartCoroutine(nameof(DestroyProjectileCoroutine));
            }
            else { GameManager.Instance.DecPowerUp(UpgradeType.Ghost); }
        }
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
        //facingDir = facingDirection;
        attackState = state;
        switch (state)
        {
            case 1:
                bounces = 1;
                break;
            case 2:
                bounces = 2;
                break;
            case 3:
                bounces = 3;
                break;
            default: //and 0

                break;
        }
    }

    private IEnumerator FlashWhite()
    {
        var prevMat = sr.material;
        var prevColor = sr.color;
        sr.material = whiteMat;
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sr.material = prevMat;
        sr.color = prevColor;
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
        sr.color = Color.white;

        var seq = DOTween.Sequence();
        seq.AppendInterval(0.1f);
        seq.Append(transform.GetChild(0).DOScale(Vector2.zero, 0.15f));
        yield return new WaitForSeconds(0.25f);

        var newBand = Instantiate(landedPrefab, transform.position, Quaternion.identity).transform;
        newBand.Rotate(0, 0, Random.Range(0, 3) * 90);

        Destroy(gameObject);
    }
}
