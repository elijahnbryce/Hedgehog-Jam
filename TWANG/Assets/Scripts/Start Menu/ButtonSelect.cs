using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private MenuSelect selectManager;

    [SerializeField] private float distance = 10f;
    [Range(0f, 2f), SerializeField] private float duration = .1f, scale = 1.1f;

    private Transform startPos;
    private float endY;
    private Tween uppies;

    void Start()
    {
        startPos = transform;
        endY = startPos.position.y + distance;
        uppies = transform.DOLocalMoveY(endY, duration)
            .Pause()
            .SetEase(Ease.InCubic)
            .SetAutoKill(false)
            .SetUpdate(true)
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    private void SelectAnim(bool select)
    {
        if (select)
        {
            uppies.Restart();
        }
        else
        {
            uppies.PlayBackwards();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        SelectAnim(true);
        selectManager.SelectButton(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SelectAnim(false);
    }

    public void SetManager(MenuSelect m)
    {
        selectManager = m;
    }
}
