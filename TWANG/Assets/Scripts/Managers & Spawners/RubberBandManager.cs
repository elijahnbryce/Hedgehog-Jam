using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RubberBandManager : MonoBehaviour
{
    [SerializeField] private List<RubberBandStruct> rubberBands = new();
    private Dictionary<RubberBandType, RubberBandStruct> bandDictionary;

    public static RubberBandManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialize dictionary
        bandDictionary = new Dictionary<RubberBandType, RubberBandStruct>();
        foreach (var band in rubberBands)
        {
            if (!bandDictionary.ContainsKey(band.BandType))
            {
                bandDictionary.Add(band.BandType, band);
            }
            else
            {
                Debug.LogWarning($"Duplicate rubber band type found: {band.BandType}");
            }
        }

        // Validate all enum values are covered
        foreach (RubberBandType type in System.Enum.GetValues(typeof(RubberBandType)))
        {
            if (!bandDictionary.ContainsKey(type))
            {
                Debug.LogError($"Missing rubber band configuration for type: {type}");
            }
        }
    }

    public RubberBandStruct GetRubberBand(RubberBandType type)
    {
        if (bandDictionary.TryGetValue(type, out RubberBandStruct band))
        {
            return band;
        }

        Debug.LogWarning($"Rubber band type {type} not found, falling back to Normal");
        return bandDictionary[RubberBandType.Normal];
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
    public RubberBandType BandType;
    public float PickupCost;
    public GameObject BandPrefab;
}