using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorSoldier : MonoBehaviour
{
    private GameObject player;
//    public GameObject target;
    public float speed;
    private float slrpSpeedDiv = 1;
	
    private Vector3 oldPos;
    private bool oldPosSet;
    private bool isSlerping;
    //public int target;
 
    //public Transform start, end;
    public AnimationCurve curve;

    private float distance;

    private float accum;

    //public PlayerMovement pm;

     
    // Start is called before the first frame update
    void Start()
    {
	//oldPosSet = false;
	//isSlerping = false;	
	player = GameObject.Find("Player");
    }
	
        // Update is called once per frame
void FixedUpdate()
    {	
	distance = Vector2.Distance(transform.position, player.transform.position);	
	
	if (distance > 20 && !oldPosSet)
		Jump();
	else
		Chase();
	
	if (isSlerping) {
		//Debug.Log("Slerp!");
		accum += Time.deltaTime;
		//accum /= slrpSpeedDiv;	
		float slerpFact = curve.Evaluate(Mathf.Clamp(accum, 0f, 1f));
		
		//if (pm.Moving)
		//	transform.position = Vector3.Slerp(transform.position, player.transform.position, slerpFact);
		//else
		transform.position = Vector3.Slerp(transform.position, oldPos, slerpFact);

		if (slerpFact >= 1.0f) {
			//Debug.Log("Stop slerping!");
			isSlerping = false;
			oldPosSet = false;
			accum = 0;
		}	
	}

	
    }
	
    void Chase()
    {
	//follow player script
	Vector2 direction = player.transform.position - transform.position;
	direction.Normalize();
	
	float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
	
	transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
	transform.rotation = Quaternion.Euler(Vector3.forward * angle);
	
    }

    void Jump()
    {
	oldPos = player.transform.position;
	oldPosSet = true;
	isSlerping = true;
		
    }
 
}
