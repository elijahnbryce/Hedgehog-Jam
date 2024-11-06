using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sticker : MonoBehaviour
{
    [SerializeField] private Transform mask;
    [SerializeField] private Transform front, back, shadow;

    [SerializeField] private float duration = 2f;

    private Tween maskT, backT;

    public void Set()
    {
        float targY = front.position.y;
        float maskOffset = Mathf.Abs(mask.position.y - back.position.y);
        float maskTarg = mask.position.y + (targY - (mask.position.y + maskOffset));
        float backTarg = back.position.y + (targY - (back.position.y + maskOffset)) * 2;

        SetSprites();

        maskT = stickerTween(mask, maskTarg);
        backT = stickerTween(back, backTarg);

        StickAnim();
    }

    private Tween stickerTween(Transform t, float d)
    {
        Tween new_t = t.DOMoveY(d, duration)
            .SetEase(Ease.InOutCubic)
            .Pause()
            .SetAutoKill(false)
            .OnRewind(() => gameObject.SetActive(false))
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        return new_t;
    }

    public void SetSprites()
    {
        Sprite s = front.GetComponent<SpriteRenderer>().sprite;
        back.GetComponent<SpriteRenderer>().sprite = s;
        shadow.GetComponent<SpriteRenderer>().sprite = s;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    StickerAnim(peel);
        //    peel = !peel;
        //}
    }

    public void StickerAnim(bool down)
    {
        if (!down)
        {
            DOTween.PlayAll();
            //maskT.Restart();
            //backT.Restart();
        }
        else
        {
            DOTween.PlayBackwardsAll();
            //maskT.PlayBackwards();
            //backT.PlayBackwards();
        }
    }

    public void StickAnim()
    {
        shadow.gameObject.SetActive(true);
        DOTween.RestartAll();
    }

    public void PeelAnim()
    {
        DOTween.PlayBackwardsAll();
    }
}
