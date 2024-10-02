using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueTrap : MonoBehaviour
{
    private GameObject player;
    private PlayerMovement pm;

    void Start()
    {
	player = GameObject.Find("Player");
	pm = player.GetComponent<PlayerMovement>(); 	
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
	
	if (other.gameObject.CompareTag("Player")) {
		pm.movementSpeed /= 2;
	}	
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) {	
        	pm.movementSpeed *= 2;
        }
    } 
}
