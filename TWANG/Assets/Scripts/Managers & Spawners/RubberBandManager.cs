using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RubberBandManager : MonoBehaviour
{
    [SerializeField] private List<RubberBandStruct> rubberBands = new();

    public static RubberBandManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public RubberBandStruct GetRubberBand(RubberBandType type)
    {
        //return rubberBands.FirstOrDefault(b => b.BandType.Equals(type));

        //i dont know if this is working
        return rubberBands[(int)type];
    }
    public float GetPickupCost(RubberBandType type)
    {
        return GetRubberBand(type).PickupCost;
    }
    public GameObject GetBandPrefab(RubberBandType type)
    {
        return GetRubberBand(type).BandPrefab;
    }
}

public enum RubberBandType
{
    Normal,
    Flaming,
}

[System.Serializable]
public struct RubberBandStruct
{
    public float PickupCost;
    public RubberBandType BandType;
    public GameObject BandPrefab;
}
