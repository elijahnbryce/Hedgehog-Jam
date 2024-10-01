using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
  Vector2 difference = Vector2.zero;

  public Transform weaponOrigin;
  public AnimationCurve curve;

  public int lrpSpeedMult;

  private bool mouseDwn = false;

  private void OnMouseDown()
  {
	difference = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;

	mouseDwn = true;
  } 

  private void OnMouseDrag()
  {
	transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - difference;

	if ((transform.position.x - weaponOrigin.position.x) > 2 || (transform.position.x - weaponOrigin.position.x) < -2
		|| (transform.position.y - weaponOrigin.position.y) > 2 || (transform.position.y - weaponOrigin.position.y) < -2
		&& !mouseDwn) {
		//Debug.Log("Stretch x");
		transform.position = Vector3.Lerp(this.transform.position, weaponOrigin.position, curve.Evaluate(Time.deltaTime) * lrpSpeedMult);
	
	}
	//if ((transform.position.y - weaponOrigin.position.y) > 2 || (transform.position.y - weaponOrigin.position.y) < -2) {
	//	Debug.Log("Stretch y");
	//	transform.position = Vector3.Lerp(transform.position, weaponOrigin.position, curve.Evaluate(Time.deltaTime) * lrpSpeedMult);
	//
	//}

  }
    
}
