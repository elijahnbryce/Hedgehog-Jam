using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[Serializable]
//example format of how states should be defined. Regarding which methods to implement, check EntityState.cs.
[CreateAssetMenu(menuName = "EntityState/DroppingGlue", fileName = "DroppingGlue")] //menuName = "EntityState/StateName"
public class DroppingGlue : EntityState //must inherit from "EntityState"
{
    public GameObject gluePrefab;

    public override void Enter()
    {
        base.Enter();
        selfEntity.StartCoroutine(IntervalDropGlue());
        
    }

    public IEnumerator IntervalDropGlue()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            Instantiate(gluePrefab, selfEntity.transform.position + new Vector3(0, -0.8f), selfEntity.transform.rotation);
            if (!isActive) { break; }
        }
    }
}

