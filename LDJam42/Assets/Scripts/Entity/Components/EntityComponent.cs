using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityComponent {

    public ComponentID componentID;

    public EntityComponent(ComponentID componentID)
    {
        this.componentID = componentID;
    }

    public abstract void Init(Entity entity, GameObject entityGO);

    public abstract void RegisterCBListener<T>(T listener);

    public abstract void UnRegisterCBListener<T>(T listener);
}



public enum ComponentID
{
    Render,
    Position,
    Fighter,
    Turn,
    AI,
    Item,
    Abilities,
    Inventory,
    Equipment,
    Weapon,
    Armor,
    Consumable,
    XP
}

