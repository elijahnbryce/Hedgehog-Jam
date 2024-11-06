using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSelect : MonoBehaviour
{
    public List<GameObject> buttons = new();

    public GameObject last;
    public int lastInx;

    private void Start()
    {
        foreach (var button in buttons)
        {
            button.GetComponent<ButtonSelect>().SetManager(this);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(SelectAfterDelay());
    }

    private IEnumerator SelectAfterDelay()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(buttons[0]);
    }

    private void Update()
    {
        HandleNavigation(Input.GetAxis("Horizontal"));
    }

    private void HandleNavigation(float a)
    {
        if (EventSystem.current.currentSelectedGameObject == null && last !=  null)
        {
            int newInx = (lastInx + Mathf.CeilToInt(a)) % buttons.Count;
            EventSystem.current.SetSelectedGameObject(buttons[newInx]);
        }
    }

    public void SelectButton(GameObject b)
    {
        last = b;
        lastInx = buttons.IndexOf(b);
    }
}
