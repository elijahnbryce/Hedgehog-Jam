using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueEnemy : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    public GameObject glue;

    private GameObject player;

    //private bool up, down, left, right;
    // Start is called before the first frame update
    void Start()
    {
	int rand = Random.Range(1, 2);

       player = GameObject.Find("Player");

       rb = GetComponent<Rigidbody2D>(); 
	
       if (rand == 1) {
       		rb.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
		rb.AddForce(Vector2.left * speed, ForceMode2D.Impulse);	
       } else if (rand == 2) {
		rb.AddForce(Vector2.down * speed, ForceMode2D.Impulse);
		rb.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
       }
	StartCoroutine(Splot());
       StartCoroutine(Kill());
    }

    // Update is called once per frame
    void Update()
    {
    	//int rand = Random.Range(0, 10);

	if (rb.velocity.magnitude > 30)
		rb.velocity *= 0.5f;

        //if (rand == 1) {
	//	//Debug.Log("1");
	//       	rb.AddForce(Vector2.up, ForceMode2D.Impulse);
	//	
	//        //Instantiate(glue, transform.position, transform.rotation);	
	//} else if (rand == 2) {
	//	//Debug.Log("2");
	//	rb.AddForce(Vector2.down, ForceMode2D.Impulse);
	//} else if (rand == 3) {
	//	//Debug.Log("3");
	//	rb.AddForce(Vector2.left, ForceMode2D.Impulse);
	//	//Instantiate(glue, transform.position, transform.rotation);
        //} else if (rand == 4) {
	//	//Debug.Log("4");
	//        rb.AddForce(Vector2.right, ForceMode2D.Impulse);
	//}
    }

    private IEnumerator Kill()
    {
	yield return new WaitForSeconds(10);
	Destroy(this.gameObject);
    }

    private IEnumerator Splot()
    {	
	Instantiate(glue, transform.position, transform.rotation);
	yield return new WaitForSeconds(2);
	StartCoroutine(Splot());	
    }
}
