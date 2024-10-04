using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PencilScribble : MonoBehaviour
{
    private Sequence scribbleS;

    [SerializeField] private Transform pencil;
    [SerializeField] private float drawSpeed = 2f;
    [SerializeField] private List<Vector2> pointList = new List<Vector2>();
    private LineRenderer line;

    public void ScribbleScrabble(List<Vector2> nodeList = null) // pass in the button instead
    {
        nodeList = nodeList ?? pointList;
        scribbleS = DOTween.Sequence().SetAutoKill(false);

        line = GetComponentInChildren<LineRenderer>();
        line.positionCount = 0;
        line.SetPosition(0, nodeList[0]);
        line.enabled = true;

        pencil.position = nodeList[0];

        for (int i = 1, len = nodeList.Count; i < len; i++)
        {
            line.positionCount = i+1;
            line.SetPosition(i, nodeList[i]);
            scribbleS.Append(pencil.DOLocalMove(nodeList[i], drawSpeed)).SetEase(Ease.InCubic);
        }
        // switch button sprite
        // line.enabled = false
    }

    public void Rewind()
    {
        scribbleS.Rewind();
    }

    private List<Vector2> PointsOnButton()
    {
        List<Vector2> pointList = new List<Vector2>();
        RectTransform rect = GetComponent<RectTransform>();
        float x = rect.rect.x, y = rect.rect.y, w = rect.rect.width, h = rect.rect.height;
        int points = 8;
        int factor = Mathf.RoundToInt(w/points);
        int startX = Mathf.RoundToInt(x - w / 2);
        int halfY = Mathf.RoundToInt(h / 2);
        for (int i = 0; i < points; i++)
        {
            if (i % 2 == 0)
            {
                pointList.Add(new Vector2(startX + factor*i + factor*0.25f, y + halfY));
            }
            else
            {
                pointList.Add(new Vector2(startX + factor*i-1 - factor*0.5f, y - halfY));
            }
        }
        return pointList;
    }
}
