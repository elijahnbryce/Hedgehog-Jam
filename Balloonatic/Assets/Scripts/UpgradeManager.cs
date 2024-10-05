using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private List<Upgrade> upgrades = new();
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

[System.Serializable]
public struct Upgrade
{
    public UpgradeType UpgradeType; //rename later lol sorry
    public Sprite UpgradeSprite;

}

public enum UpgradeType
{
    Health,
    MoveSpeed,
    Strength,
}
