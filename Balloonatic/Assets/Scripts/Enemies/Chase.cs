using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : MonoBehaviour
{	
	public float speed;
	
	private float distance;
	private GameObject player;
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
    }

    void ChaseYou()
    {
	Vector2 direction = player.transform.position - transform.position;
	direction.Normalize();

	float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

	transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
	transform.rotation = Quaternion.Euler(Vector3.forward * angle);
    }
}
