using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : MonoBehaviour
{
    //this should prob be scriptableobject, sorry squad
    [SerializeField] private Vector2 size = Vector2.one;
    [HideInInspector] public Vector2 Size {  get { return size; } }
    void Start()
    {
        foreach (Transform child in transform)
        {
            if(UnityEngine.Random.value < 0.2f)
            {
                child.GetComponent<SpriteRenderer>().sprite = EraserManager.Instance.GetCrackedSprite(GetNumberFromString(child.GetComponent<SpriteRenderer>().sprite.name));
            }
            var colliderObj = new GameObject();
            var indicatorObj = new GameObject();

            colliderObj.transform.parent = child;
            indicatorObj.transform.parent = child;
            colliderObj.transform.localPosition = Vector2.zero;
            indicatorObj.transform.localPosition = Vector2.up * 0.2f;

            //colliderObj.layer = LayerMask.NameToLayer("Wall");
            colliderObj.tag = "Wall";
            //hehe
            colliderObj.name = "coll";
            indicatorObj.name = "indicator";
            var sr = colliderObj.AddComponent<SpriteRenderer>();
            var sr2 = indicatorObj.AddComponent<SpriteRenderer>();

            child.GetComponent<SpriteRenderer>().sortingLayerName = "Environment";
            sr.sprite = EraserManager.Instance.GetColliderSprite(GetNumberFromString(child.GetComponent<SpriteRenderer>().sprite.name));
            sr2.sprite = EraserManager.Instance.GetIndicatorSprite(GetNumberFromString(child.GetComponent<SpriteRenderer>().sprite.name));
            
            sr.enabled = false;

            colliderObj.AddComponent<PolygonCollider2D>();

            child.gameObject.AddComponent<UnityEngine.Rendering.SortingGroup>().sortingLayerName = "Player";
        }

        transform.parent = EraserManager.Instance.transform;
    }

    public static int GetNumberFromString(string input)
    {
        int underscoreIndex = input.LastIndexOf('_');

        if (underscoreIndex == -1 || underscoreIndex == input.Length - 1)
        {
            Debug.LogError("oops");
            return -1;
        }

        string numberPart = input.Substring(underscoreIndex + 1);

        if (int.TryParse(numberPart, out int result))
        {
            return result;
        }
        else
        {
            Debug.LogError("oops");
            return -1;
        }
    }
}
