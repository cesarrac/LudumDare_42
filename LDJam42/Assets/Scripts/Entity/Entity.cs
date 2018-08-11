using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity  {

    public EntityType entityType { get; protected set; }
    public string Name { get; protected set; }
    EntityComponent[] components;
    public Action<Entity> InteractCB;

    public Entity(string name, EntityType eType)
    {
        Name = name;
        entityType = eType;
    }
    public Entity(string name, EntityType eType, EntityComponent[] components)
    {
        Name = name;
        this.components = components;
        entityType = eType;
    }

    public void InitComponent(GameObject entityGO)
    {
        for (int i = 0; i < components.Length; i++)
        {
            components[i].Init(this, entityGO);
        }
    }
    public EntityComponent GetEntityComponent(ComponentID id)
    {
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i].componentID == id)
                return components[i];
        }
        return null;
    }
}


public enum EntityType
{
    Unit,
    Item
}