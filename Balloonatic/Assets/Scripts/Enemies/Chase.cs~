using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : MonoBehaviour
{	
	public float speed;
	public AnimationCurve curve;
	
	private float distance;
	private GameObject player;

	private Vector3 oldPos;
	private bool oldPosSet;
	private bool isSlerping;
	private float accum;
    // Start is called before the first frame update
    void Start()
    {
       player = GameObject.Find("Player"); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       distance = Vector2.Distance(transform.position, player.transform.position);

       ChaseYou();

       if (distance <= 5 && !oldPosSet)
	      Jump();

       if (isSlerping) {
		accum += Time.deltaTime;

		float slerpFact = curve.Evaluate(Mathf.Clamp(accum, 0f, 1f));

		transform.position = Vector3.Slerp(transform.position, oldPos, slerpFact);

		if (slerpFact >= 1.0f) {
			isSlerping = false;
			oldPosSet = false;
			accum = 0;
		}
       } 
    }

    void ChaseYou()
    {
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
