using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sticker : MonoBehaviour
{
    [SerializeField] private bool peel = false;
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

        maskT = mask.transform.DOMoveY(maskTarg, duration).SetEase(Ease.InOutCubic).Pause().SetAutoKill(false).OnRewind(()=>gameObject.SetActive(false));
        backT = back.transform.DOMoveY(backTarg, duration).SetEase(Ease.InOutCubic).Pause().SetAutoKill(false).OnRewind(()=>gameObject.SetActive(false));

        StickAnim();
        peel = true;
        //maskT = mask.DOMove(new Vector2(mask.position.x + (front.position.x - (mask.position.x + maskOffset)), maskTarg), duration)
        //    .SetEase(Ease.InOutCubic).Pause().SetAutoKill(false);
        //backT = back.DOMove(new Vector2(back.position.x + (front.position.x - (back.position.x + maskOffset)) * 2, backTarg), duration)
        //    .SetEase(Ease.InOutCubic).Pause().SetAutoKill(false);
    }

    public void SetSprites()
    {
        Sprite s = front.GetComponent<SpriteRenderer>().sprite;
        back.GetComponent<SpriteRenderer>().sprite = s;
        shadow.GetComponent<SpriteRenderer>().sprite = s;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            StickerAnim(peel);
            peel = !peel;
        }
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
