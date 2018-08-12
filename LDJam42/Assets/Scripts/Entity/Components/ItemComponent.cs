using UnityEngine;
using System.Collections;

public class ItemComponent : EntityComponent
{
    Entity thisEntity;
    public ItemComponent(string itemName) : base(ComponentID.Item)
    {
    }

    public override void Init(Entity entity, GameObject entityGO)
    {
        throw new System.NotImplementedException();
    }

    void PickUp()
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
