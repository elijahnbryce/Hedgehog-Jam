using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Windows;

[Serializable]
[CreateAssetMenu(menuName = "EntityState/TackChrysalis", fileName = "TackChrysalis")]
public class TackChrysalis : EntityState
{
    public List<GameObject> tacks = new();

    public override void Enter()
    {
        base.Enter();
        selfEntity.StartCoroutine(Hatch());
    }

    public IEnumerator Hatch()
    {
        var time = 0.2f * 8 * 4;
        yield return new WaitForSeconds(time);

        var num = int.Parse(Regex.Match(selfEntity.gameObject.name, @"\d").Value) - 1;

        GameObject newTack = Instantiate(tacks[num], selfEntity.transform.position, selfEntity.transform.rotation);
        GameManager.Instance.AddEnemy(newTack);

        GameManager.Instance.RemoveEnemy(selfEntity.gameObject);

        yield break;
    }
}