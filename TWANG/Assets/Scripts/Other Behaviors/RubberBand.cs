using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using System.IO.Pipes;
using Unity.Burst.CompilerServices;

public class RubberBand : MonoBehaviour
{
    [SerializeField] private List<Sprite> rubberBandSprites = new();
    [SerializeField] Sprite landedSprite;
    [SerializeField] Material whiteMat;
    [SerializeField] LayerMask boundaryLayer;

    private bool landed = false;
    private Material defaultMat;
    private SpriteRenderer sr;
    private int _maxBounces = 0;

    public float attackPower = 1f;

    private Rigidbody2D rb;
    void Awake()
    {
        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        defaultMat = sr.material;
    }

    void Update()
    {
        CheckBoundary();
        if (landed) return;
        UpdateRubberBandSprite();
    }

    void CheckBoundary()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rb.velocity, 1f, boundaryLayer.value);
        if (hit.collider != null)
        {
            ProjectileLand();
        }
    }

    public void InitializeProjectile(Vector2 dir, int maxBounces = 1)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        landed = false;
        seq.Kill();
        rb.isKinematic = false;

        rb.AddForce(dir);
        _maxBounces = maxBounces;
    }

    void UpdateRubberBandSprite()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;

        // Normalize angle to positive
        if (angle < 0) angle += 360f;

        if (angle > 180f)
            angle -= 180f;

        int spriteIndex = 4;

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

        // Update the sprite
        if (spriteIndex < rubberBandSprites.Count)
        {
            sr.sprite = rubberBandSprites[spriteIndex];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.transform.tag)
        {
            case "Player":
                Debug.Log("Band hit player");
                this.gameObject.SetActive(false);
                PlayerAttack.Instance.PickupBand();
                break;
            case "Wall":
                if (landed) break;

                SoundManager.Instance.PlaySoundEffect("band_hit");
                CameraManager.Instance.ScreenShake();

                // Bounce
                if (_maxBounces <= 0)
                    ProjectileLand();
                if (_maxBounces > 0)
                {
                    transform.DOShakeScale(0.1f);
                    _maxBounces--;

                    SeekEnemies();
                }
                break;
            case "Level Boundary":
                SoundManager.Instance.PlaySoundEffect("band_hit");
                CameraManager.Instance.ScreenShake();
                ProjectileLand();
                break;
            case "Enemy":
                if (landed) break;

                if (collision.transform.TryGetComponent(out Entity e))
                {
                    e.stats.TakeDamage((int)attackPower);
                    ProjectileLand();
                }

                //is this dumb?
                else if (_maxBounces <= 0)
                    ProjectileLand();
                else if (_maxBounces > 0)
                {
                    transform.DOShakeScale(0.1f);
                    _maxBounces--;

                    SeekEnemies();
                }

                //if (!GameManager.Instance.upgradeList.ContainsKey(UpgradeType.Ghost))
                //{
                //    ProjectileLand();
                //}
                //else
                //{
                //    GameManager.Instance.DecPowerUp(UpgradeType.Ghost);
                //}
                break;
        }
    }

    Sequence seq;
    private void ProjectileLand()
    {
        if (landed) return;

        landed = true;

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        sr.material = whiteMat;
        sr.transform.localScale = Vector3.one;
        seq = DOTween.Sequence();
        seq.AppendInterval(0.1f);
        seq.Append(sr.transform.DOScale(Vector2.zero, 0.15f));
        seq.AppendCallback(() =>
        {
            sr.sprite = landedSprite;
        });
        seq.Append(sr.transform.DOScale(Vector3.one, 0.15f));
        seq.AppendCallback(() =>
        {
            sr.material = defaultMat;
        });

        seq.Play();
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
                rb.velocity = (collider.transform.position - transform.position).normalized * 15;
            }
        }

        return found;
    }
}
