using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EntityManager : MonoBehaviour
{
    public static EntityManager instance { get; protected set; }
    Dictionary<string, Entity> EntityProtoMap;
    Dictionary<Entity, GameObject> EntityGOMap;
    List<Entity> Entities;

    public Func<MoveData> GetPlayerPositionData;

    ObjectPool pool;

    private void Awake()
    {
        instance = this;
        Entities = new List<Entity>();
        EntityGOMap = new Dictionary<Entity, GameObject>();
        EntityProtoMap = new Dictionary<string, Entity>();

        //Entity playerProto = new Entity("Player", EntityType.Unit,
        //                                new EntityComponent[]
        //                                {
                                            
        //                                });

        EntityProtoMap.Add("Player", new Entity("Player", EntityType.Unit));
    }

    public void SpawnEntity(string entityProtoName, Vector2 position, bool isPlayer = false)
    {
        if (pool == null)
        {
            pool = ObjectPool.instance;
        }
        if (EntityProtoMap.ContainsKey(entityProtoName) == false)
        {
            Debug.LogError("EntityManager could not find a protoype called " + entityProtoName + " and cannot spawn it!");
            return;
        }

        Entity proto = EntityProtoMap[entityProtoName];

        EntityComponent[] components = new EntityComponent[]
        {
            new RenderComponent(entityProtoName),
            new PositionComponent(position.x, position.y),
            new FighterComponent(10, 10)
        };

        GameObject entityGO = pool.GetObjectForType("Entity", true, position);
        if (entityGO == null)
        {
            // Make a new one?
            return;
        }

        Entity newEntity = new Entity(proto.Name, proto.entityType, components);

        newEntity.InitComponent(entityGO);

        EntityGOMap.Add(newEntity, entityGO);
        Entities.Add(newEntity);

        if (isPlayer == true)
        {
            PositionComponent posC = (PositionComponent)newEntity.GetEntityComponent(ComponentID.Position);
            // Register input callbacks
            PlayerInputSystem.instance.RegisterOnInputCB(posC.Move);

            GetPlayerPositionData += posC.GetPositionData;
        }
    }

}
