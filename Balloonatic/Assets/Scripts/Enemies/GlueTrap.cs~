using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueTrap : MonoBehaviour
{
    private GameObject player;
    private PlayerMovement pm;
    private Vector3 scaleChange;

    private bool isScaling;

    void Start()
    {
	player = GameObject.Find("Player");
	pm = player.GetComponent<PlayerMovement>(); 

	isScaling = true;

	scaleChange = new Vector3(0.1f, 0.1f, 0.1f);	
    }

    void FixedUpdate()
    {
	if (isScaling) {
		transform.localScale += scaleChange;
		
		if (transform.localScale.y > 6)
			isScaling = false;
	}

	StartCoroutine(Life());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
	
	if (other.gameObject.CompareTag("Player")) {
		pm.movementSpeed /= 3;
	}	
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) {	
        	pm.movementSpeed *= 3;
        }
    } 

    private IEnumerator Life()
    {
	yield return new WaitForSeconds(1);
	Destroy(this.gameObject);
    }
}
