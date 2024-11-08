using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PauseUI : MonoBehaviour
{
    [Header("Animating Elements")]
    [SerializeField] GameObject _pauseMenu;
    [SerializeField] float _animateTime = 1f;
    [SerializeField] float _animateOutTime = 0.3f;
    [SerializeField] RectTransform _timeoutTitle;
    [SerializeField] RectTransform[] _pauseButtonImages;

    bool _pauseState;

    const float offscreenTitleY = 350f;
    const float pauseButtonY = -350f;

    // Start is called before the first frame update
    void OnEnable()
    {
        EventHandler.onPauseUpdate += PauseUpdate;
    }
    void OnDisable()
    {
        EventHandler.onPauseUpdate -= PauseUpdate;
    }

    void ResetPositions()
    {
        // Reset positions of UI elements
        _timeoutTitle.anchoredPosition = new Vector3(0, offscreenTitleY);
        for (int i = 0; i < _pauseButtonImages.Length; i++)
        {
            _pauseButtonImages[i].anchoredPosition = new Vector3(0, pauseButtonY);
        }
    }

    Sequence openSeq;
    void PauseUpdate(bool pauseState)
    {
        _pauseState = pauseState;

        if (_pauseState)
        {
            OnPauseAnim();
        }
        else
        {
            OnUnpauseAnim();
        }
    }

    void OnPauseAnim()
    {
        ResetPositions();
        _pauseMenu.SetActive(true);
        if (openSeq != null)
        {
            openSeq.Kill();
        }

        openSeq = DOTween.Sequence();
        openSeq.SetUpdate(true);
        openSeq.Insert(0, _timeoutTitle.DOAnchorPosY(0, _animateTime));
        for (int i = 0; i < _pauseButtonImages.Length; i++)
        {
            openSeq.Insert((_animateTime / (float)_pauseButtonImages.Length) * i, _pauseButtonImages[i].DOAnchorPosY(0, _animateTime));
        }


        openSeq.Play();
    }

    void OnUnpauseAnim()
    {
        if (openSeq != null)
        {
            openSeq.Kill();
        }

        openSeq = DOTween.Sequence();
        openSeq.SetUpdate(true);
        openSeq.Insert(0, _timeoutTitle.DOAnchorPosY(offscreenTitleY, _animateOutTime));
        for (int i = 0; i < _pauseButtonImages.Length; i++)
        {
            openSeq.Insert((_animateOutTime / (float)_pauseButtonImages.Length) * (_pauseButtonImages.Length - 1 - i), _pauseButtonImages[i].DOAnchorPosY(pauseButtonY, _animateOutTime));
        }
        openSeq.Play();

        openSeq.onComplete = () =>
        {
            _pauseMenu.SetActive(false);
        };
    }
}
