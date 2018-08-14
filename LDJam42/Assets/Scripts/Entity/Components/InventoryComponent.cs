using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : EntityComponent
{
    int maxSpaces = 10;
    ItemComponent[] items;
    PositionComponent positionComponent;
    EquipmentComponent equipment;
    Entity thisEntity;
    public InventoryComponent() : base(ComponentID.Inventory)
    {
       
    }

    public override void Init(Entity entity, GameObject entityGO)
    {
        items = new ItemComponent[maxSpaces];
        positionComponent = (PositionComponent)entity.GetEntityComponent(ComponentID.Position);
        equipment = (EquipmentComponent)entity.GetEntityComponent(ComponentID.Equipment);
        thisEntity = entity;
    }

    public bool AddItem(ItemComponent item)
    {
        //int emptyIndex = FindEmptyIndex();
        //if (emptyIndex < 0)
        //    return false;
        if (item.itemType == ItemType.Armor || item.itemType == ItemType.Weapon)
        {
            equipment.AddEquipment(item);
            return true;
        }
        else
        {
            FighterComponent fighterComponent = (FighterComponent)thisEntity.GetEntityComponent(ComponentID.Fighter);
            HealthDropComponent consumable = (HealthDropComponent)item.thisEntity.GetEntityComponent(ComponentID.Consumable);
            fighterComponent.GainHealth(consumable.HealthGained);
            MessageLog_Manager.NewMessage("You consume " + item.itemName, Color.green);
            return true;
        }

        //items[emptyIndex] = item;
        //Debug.Log(item.itemName + " added to inventory at index " + emptyIndex);
        //MessageLog_Manager.NewMessage("You pick up " + item.itemName, Color.yellow);
        //return true;
    }
    public void Drop(int index)
    {
        Debug.Log("Calling drop with index " + index);
        if (index < 0 || index >= items.Length)
            return;
        if (items[index] == null)
            return;
        items[index].Drop(positionComponent.moveData);
        items[index] = null;
    }

    int FindEmptyIndex()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
                return i;
        }
        return -1;
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
