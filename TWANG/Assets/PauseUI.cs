using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [Header("Pause Menu Buttons")]
    [SerializeField] Button[] _hoverElements;

    [Header("Animating Elements")]
    [SerializeField] float _animateTime = 1f;
    [SerializeField] RectTransform _timeoutTitle;
    [SerializeField] RectTransform[] _pauseButtonImages;

    int _selected = 0;
    bool _pauseState;

    // Start is called before the first frame update
    void OnEnable()
    {
        EventHandler.onPauseUpdate += PauseUpdate;
    }
    void OnDisable()
    {
        EventHandler.onPauseUpdate -= PauseUpdate;
    }

    private void Update()
    {
        if (!_pauseState) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            _selected--;
            if (_selected < 0)
            {
                _selected = _hoverElements.Length - 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _selected++;
            if (_selected >= _hoverElements.Length)
            {
                _selected = 0;
            }
        }
        _hoverElements[_selected].Select();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _hoverElements[_selected].onClick?.Invoke();
        }
    }

    void ResetPositions()
    {
        // Reset positions of UI elements
        _timeoutTitle.anchoredPosition = new Vector3(0, 350);
        for (int i = 0; i < _pauseButtonImages.Length; i++)
        {
            _pauseButtonImages[i].anchoredPosition = new Vector3(0, -350);
        }
    }

    Sequence openSeq;
    void PauseUpdate(bool pauseState)
    {
        _pauseState = pauseState;
        if (_selected < 0 || _selected >= _hoverElements.Length || _hoverElements.Length == 0) return;
        _selected = 0;

        ResetPositions();

        if(openSeq != null)
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

        _hoverElements[_selected].Select();
    }
}
