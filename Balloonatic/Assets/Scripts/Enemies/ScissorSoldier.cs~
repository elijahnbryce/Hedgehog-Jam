using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorSoldier : MonoBehaviour
{
    public GameObject player;
//    public GameObject target;
    public float speed;
    public float slrpSpeedMult;

    public int target;
 
    //public Transform start, end;
    public AnimationCurve curve;

    private float distance;

    //private Vector3 storedPosition;
    public PlayerMovement pm;
 
    // Start is called before the first frame update
    void Start()
    {
	target = 20;
	
    }
	
        // Update is called once per frame
void FixedUpdate()
    {	
	distance = Vector2.Distance(transform.position, player.transform.position);	
	
	if (distance > target)
		Jump();
	else
		Chase();

    }
	
    void Chase()
    {
	target = 20;    	
	//follow player script
	Vector2 direction = player.transform.position - transform.position;
	direction.Normalize();
	
	float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

	transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
	transform.rotation = Quaternion.Euler(Vector3.forward * angle);
	
    }

    void Jump()
    {
	//jump at player
	if (!pm.Moving)
		target = 2;
	else
		target = 11;
	transform.position = Vector3.Slerp(transform.position, player.transform.position, curve.Evaluate(Time.deltaTime) * slrpSpeedMult);
	
    }
 
}
