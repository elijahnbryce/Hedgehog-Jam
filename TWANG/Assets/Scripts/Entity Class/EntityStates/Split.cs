using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/Split", fileName = "Split")]

public class Split : EntityState 
{
    // Start is called before the first frame update
    public GameObject smaller;

    public override void Enter()
    {	    
	Vector3 offset = new Vector3(4, 4, 0);
	
	Instantiate(smaller, selfEntity.transform.position + offset, selfEntity.transform.rotation);
	Instantiate(smaller, selfEntity.transform.position, selfEntity.transform.rotation); 

	Destroy(selfEntity.gameObject);
    }
}
