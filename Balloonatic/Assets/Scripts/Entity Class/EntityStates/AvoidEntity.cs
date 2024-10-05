using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/AvoidEntity", fileName = "Split")]

public class AvoidEntity : EntityState 
{
	public override void Enter()
	{
		StartCoroutine(StartAvoid());
	}

	public IEnumerator StartAvoid()
	{

	}	
}
