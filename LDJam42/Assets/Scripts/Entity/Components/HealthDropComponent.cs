using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDropComponent : EntityComponent
{
    public int healthGained { get; protected set; }
    public HealthDropComponent(int healthGain) : base(ComponentID.Consumable)
    {
        healthGained = healthGain;
    }

    public override void Init(Entity entity, GameObject entityGO)
    {
        
    }

    public override void RegisterCBListener<T>(T listener)
    {
        throw new System.NotImplementedException();
    }

    public override void UnRegisterCBListener<T>(T listener)
    {
        throw new System.NotImplementedException();
    }
}
