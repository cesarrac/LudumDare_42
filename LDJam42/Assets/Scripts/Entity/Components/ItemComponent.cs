using UnityEngine;
using System.Collections;

public enum ItemType { Consumable, Weapon, Armor }
public class ItemComponent : EntityComponent
{
    public Entity thisEntity { get; protected set; }
    public MoveData positionData;
    public ItemType itemType { get; protected set; }
    public string itemName { get; protected set; }

    public ItemComponent(string itemName, ItemType itemType) : base(ComponentID.Item)
    {
        this.itemName = itemName;
        this.itemType = itemType;
    }

    public override void Init(Entity entity, GameObject entityGO)
    {
        thisEntity = entity;
        if (entityGO == null || entityGO.activeSelf == false)
            return;
        EntityActionManager.instance.EntityOnTileChanged(entity, positionData);
        entity.OnActiveChanged = (isActive) => ItemSystem.instance.OnItemActiveChanged(thisEntity, entityGO, isActive, GetPositionData());
    }

    MoveData GetPositionData()
    {
        return this.positionData;
    }

    public void PickUp()
    {
        thisEntity.ChangeActiveStatus(false);
    }
    public void Drop(MoveData position)
    {
        Debug.Log("Calling Drop on the Item component");
        this.positionData = new MoveData(position.X, position.Y);
        thisEntity.ChangeActiveStatus(true);
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
