using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
[CreateAssetMenu(menuName = "EntityState/DroppingGlue", fileName = "DroppingGlue")]
public class DroppingGlue : EntityState
{
    public GameObject gluePrefab;
    private float minDropHeight;

    public override void Enter()
    {
        base.Enter();
        selfEntity.StartCoroutine(IntervalDropGlue());
        minDropHeight = GameManager.Instance.GetSpawnBounds()[0][1] + 1; //offset fixes issue
    }

    public IEnumerator IntervalDropGlue()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);

            if (selfEntity.transform.position.y >= minDropHeight)
            {
                Instantiate(gluePrefab, selfEntity.transform.position + new Vector3(0, -0.8f), selfEntity.transform.rotation);
            }

            if (!isActive) { break; }
        }
    }
}