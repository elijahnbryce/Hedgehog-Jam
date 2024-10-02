using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
  Vector2 difference = Vector2.zero;

  public int distance;
  public Transform weaponOrigin;
  public AnimationCurve curve;

  public int lrpSpeedMult;
 
  private Vector3 offset = new Vector3 (2, 2, 0);
  private bool isReturning;	

  void Start()
  {
	isReturning = false;
  }

  void FixedUpdate()
  {
	if (isReturning == true) 
		transform.position = Vector3.Lerp(this.transform.position, weaponOrigin.position, curve.Evaluate(Time.deltaTime) * lrpSpeedMult);
	if (transform.position == weaponOrigin.position)
		isReturning = false;
  }

  private void OnMouseDown()
  {
	difference = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;

  } 

  private void OnMouseDrag()
  {
	transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - difference;

	if ((transform.position.x - weaponOrigin.position.x) > distance || (transform.position.x - weaponOrigin.position.x) < -distance
		|| (transform.position.y - weaponOrigin.position.y) > distance || (transform.position.y - weaponOrigin.position.y) < -distance) {
		//Debug.Log("Has let go out of bounds");
		
		isReturning = true;			
			
	}
	
  }
    
}
