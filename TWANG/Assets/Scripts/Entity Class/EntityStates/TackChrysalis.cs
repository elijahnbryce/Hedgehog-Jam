using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Windows;

[Serializable]
[CreateAssetMenu(menuName = "EntityState/TackChrysalis", fileName = "TackChrysalis")] 
public class TackChrysalis : EntityState //must inherit from "EntityState"
{
    public List<GameObject> tacks = new();

    public override void Enter()
    {
        base.Enter();
        selfEntity.StartCoroutine(Hatch());
    }

    public IEnumerator Hatch()
    {
        while (true)
        {
            //yield return new WaitForSeconds(2);
            var time = 0.2f * 8 * 3;
            yield return new WaitForSeconds(time);

            var num = int.Parse(Regex.Match(selfEntity.gameObject.name, @"\d").Value) - 1;
            //this is ass code fix later

            Instantiate(tacks[num], selfEntity.transform.position, selfEntity.transform.rotation);

            GameManager.Instance.RemoveEnemy(selfEntity.gameObject);
        }
    }
}

