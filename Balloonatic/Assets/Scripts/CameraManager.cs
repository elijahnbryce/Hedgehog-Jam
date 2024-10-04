using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CameraManager : MonoBehaviour
{
    private Transform playerTransform;
    private float currentZoom, targetZoom;

    public static CameraManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    void Start()
    {
        currentZoom = targetZoom = 5;
        playerTransform = PlayerMovement.Instance.transform;
        PlayerAttack.OnAttackInitiate += AttackStart;
        PlayerAttack.OnAttackHalt += AttackEnd;
    }

    void Update()
    {
        var newPos = Vector2.Lerp(transform.localPosition, playerTransform.position, 5f * Time.deltaTime);
        //ass code fix later
        transform.localPosition = new Vector3(newPos.x, newPos.y, -10);

        //transform.localScale = Vector2.one * currentZoom;

        //ass code fix later
        //GetComponent<Camera>().orthographicSize = currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * 0.5f);
        //GetComponent<PixelPerfectCamera>().assetsPPU = Mathf.RoundToInt(100 / GetComponent<Camera>().orthographicSize);
    }

    private void AttackStart()
    {
        targetZoom = 3f;
    }

    private void AttackEnd()
    {
        targetZoom = 5f;
    }

    public void ScreenShake() => ScreenShake(0.25f);
    public void ScreenShake(float amount)
    {
        //cleanup later
        transform.parent.DOShakePosition(0.15f, amount).OnComplete(() => transform.parent.localPosition = Vector3.zero);
    }

}
