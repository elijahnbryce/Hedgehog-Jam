using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class PauseUI : MonoBehaviour
{
    public static PauseUI Instance;
    public static Action ToggleGamePause;
    public static Action<bool> SetPauseState;

    public bool IsGamePaused => _isGamePaused;

    [Header("Animating Elements")]
    [SerializeField] GameObject _pauseMenu;
    [SerializeField] float _animateTime = 1f;
    [SerializeField] float _animateOutTime = 0.3f;
    [SerializeField] RectTransform _timeoutTitle;
    [SerializeField] RectTransform[] _pauseButtonImages;

    bool _isGamePaused;

    const float offscreenTitleY = 350f;
    const float pauseButtonY = -350f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        ToggleGamePause = TogglePause;
        SetPauseState = SetPause;
    }

    private void Start()
    {
        PauseUpdate();
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

    public void SetPause(bool pause)
    {
        _isGamePaused = pause;
        PauseUpdate();
    }

    void TogglePause()
    {
       _isGamePaused = !_isGamePaused;
        PauseUpdate();
    }

    Sequence openSeq;
    void PauseUpdate()
    {
        if (GameManager.Instance.gameOver) return;
        if (_isGamePaused)
        {
            Time.timeScale = 0;
            SoundManager.Instance.SwitchToMenuMusic();
            OnPauseAnim();
        }
        else
        {
            // Stop player shooting on unpause
            PlayerAttack.Instance.AddAttackCooldown(0.1f);
            Time.timeScale = 1;
            SoundManager.Instance.SwitchToRegularMusic();
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
