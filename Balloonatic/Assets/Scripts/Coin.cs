using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//some redundant animation code, fix later
public class Coin : MonoBehaviour
{
    float timer = 0.2f;
    int frame = 0;
    int value;
    List<Sprite> sprites = new();
    SpriteRenderer sr;
    [SerializeField] private Material whiteMat;
    //scriptableobject should be used here instead
    public void InitializeCoin(CoinStruct coin)
    {
        value = (int)coin.Type;
        sprites = coin.Sprites;

        transform.localScale = Vector2.zero;
        transform.DOScale(Vector2.one, 0.25f);
        frame = Random.Range(0, sprites.Count);
    }
    void Start()
    {
        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 0.2f;
            frame = (frame + 1) % 4;
        }

        sr.sprite = sprites[frame];
    }

    public void ClaimCoin()
    {
        Destroy(GetComponent<Collider2D>());
        sr.material = whiteMat;
        transform.DOMove(transform.position + Vector3.up, 0.4f);
        var seq = DOTween.Sequence();
        seq.Append(transform.DOPunchScale(Vector2.one * 0.25f, 0.2f));
        seq.Append(transform.DOScale(Vector2.zero, 0.2f));
        seq.AppendCallback(() =>
        {
            Destroy(gameObject);
            SoundManager.Instance.PlaySoundEffect("coin_pickup");
            GameManager.Instance.UpdateScore(value);
        });
    }
}
