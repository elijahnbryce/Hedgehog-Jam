using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardUIButtons : MonoBehaviour
{
    [SerializeField] Button[] _hoverElements;
    int _selected = 0;

    private void OnEnable()
    {
        _hoverElements[_selected].Select();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _selected--;
            if (_selected < 0)
            {
                _selected = _hoverElements.Length - 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
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
}
