using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EntityType", menuName = "EntityType")]
public class EntityType : ScriptableObject
{
    [HideInInspector] public Entity selfEntity;

    [Tooltip("What layers can effect--aside from physical contact--this entity?")]
    public List<LayerMask> effectorLayerMasks = new List<LayerMask>();

    public virtual void Initialize(Entity thisEntity)
    {
        selfEntity = thisEntity;
    }

    public virtual void OnTriggerEnter2D(Collider2D collider)
    {

    }
}
