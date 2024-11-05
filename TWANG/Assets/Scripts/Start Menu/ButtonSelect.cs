using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private float distance = 10f;
    [Range(0f, 2f), SerializeField] private float duration = .1f, scale = 1.1f;

    private Transform startPos;

    void Start()
    {
        startPos = transform;
    }

    private IEnumerator SelectAnim(bool select)
    {
        Vector3 endPos;
        Vector3 endScale;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (select)
            {
                endPos = startPos.position + new Vector3(0f, distance, 0f);
                endScale = startPos.localScale * scale;
            }
            else
            {
                endPos = startPos.position;
                endScale = startPos.localScale;
            }

            transform.position = Vector3.Lerp(transform.position, endPos, (elapsedTime / duration));
            transform.localScale = Vector3.Lerp(transform.localScale, endScale, (elapsedTime / duration));
        }

        yield return null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.selectedObject = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventData.selectedObject = null;
    }

    public void OnSelect(BaseEventData eventData)
    {
        StartCoroutine(SelectAnim(true));
        MenuSelect._Instance.SelectButton(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        StartCoroutine(SelectAnim(false));
    }
}
