using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    float timer = 1f;
    void Start()
    {

    }

    void Update()
    {

    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals("Player"))
        {
            Debug.Log("enter");

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Debug.Log("sakfdj;alskdfj");
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals("Player"))
        {
            Debug.Log("exit");
            timer = 1;
        }    
    }
}
