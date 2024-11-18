using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PencilScribble : MonoBehaviour
{
    [SerializeField] private Transform pencil;
    [SerializeField] private LineRenderer line;
    [SerializeField] private float drawSpeed = 2f;
    [SerializeField] private Vector3[] pointList;
    [SerializeField] private Button butt;
    private int pathIndex;
    private bool isDrawing;

    public void SquiggleSquaggle(Button button, System.Action CallAfter)
    {
        pointList = GetChildTrasforms(button);

        UpdatePath(0);
        line.SetPosition(0, pointList[0]);
        line.enabled = isDrawing = true;

        pencil.position = pointList[0];
        pencil.gameObject.SetActive(true);
        pencil
            .DOPath(pointList, drawSpeed, PathType.CatmullRom, PathMode.Ignore, gizmoColor: null)
            .SetEase(Ease.InOutSine)
            .OnWaypointChange(UpdatePath)
            .OnComplete(()=> isDrawing = false)
            .OnComplete(()=> CallAfter());

        // switch button sprite
        // line.enabled = false;
    }
    void UpdatePath(int waypointIndex)
    {
        //Debug.Log("Waypoint index changed to " + waypointIndex);
        pathIndex = waypointIndex + 1;
        line.positionCount = waypointIndex + 2;
    }

    private void Start()
    {
        line.enabled = isDrawing = false;
        pencil.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDrawing)
        { line.SetPosition(pathIndex, pencil.position); }
    }

    private Vector3[] GetChildTrasforms(Button button)
    {
        Transform t = button.transform.GetChild(1);
        Debug.Log(t);

        int len = t.childCount;
        Vector3[] pointList = new Vector3[len];

        for (int i = 0; i < len; i++)
        {
            Transform point = t.GetChild(i);
            Debug.Log(point.position);
            pointList[i] = new Vector3(point.position.x + Random.Range(-0.07f, 0.07f), point.position.y + Random.Range(-0.03f, 0.03f), 0f);
        }
        return pointList;
    }
}
