using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EntityManager : MonoBehaviour
{
    public static EntityManager instance { get; protected set; }
    Dictionary<string, Entity> EntityProtoMap;
    Dictionary<Entity, GameObject> EntityGOMap;
    Entity[] Enemies;
    Entity Player;
    List<Entity> Items;

    public Func<MoveData> GetPlayerPositionData;

    ObjectPool pool;

    private void Awake()
    {
        instance = this;
        Items = new List<Entity>();
        EntityGOMap = new Dictionary<Entity, GameObject>();
        EntityProtoMap = new Dictionary<string, Entity>();

        //Entity playerProto = new Entity("Player", EntityType.Unit,
        //                                new EntityComponent[]
        //                                {
                                            
        //                                });

        EntityProtoMap.Add("Player", new Entity("Player", EntityType.Unit));
        EntityProtoMap.Add("Enemy", new Entity("Enemy", EntityType.Unit));
    }

    public void SpawnPlayer(Vector2 position)
    {
        if (pool == null)
        {
            pool = ObjectPool.instance;
        }

        Entity proto = EntityProtoMap["Player"];

        EntityComponent[] components = new EntityComponent[]
        {
            new RenderComponent("Player"),
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

        newEntity.CanEndTurnCB = CanPlayerEndTurn;

        PositionComponent posC = (PositionComponent)newEntity.GetEntityComponent(ComponentID.Position);
        // Register input callbacks
        PlayerInputSystem.instance.RegisterOnInputCB(posC.Move);

        GetPlayerPositionData += posC.GetPositionData;

        Player = newEntity;

    }

    public void SpawnEnemies(Vector2[] positions)
    {
        if (pool == null)
        {
            pool = ObjectPool.instance;
        }
        //if (positions.Length != count)
        //{
        //    Debug.Log("Trying to spawn more enemies than there are positions! ... FIXING Enemies array length");
        //    count = positions.Length;
        //}
        if (EntityProtoMap.ContainsKey("Enemy") == false)
        {
            Debug.LogError("EntityManager could not find a protoype called " + "Enemy" + " and cannot spawn it!");
            return;
        }

        Entity proto = EntityProtoMap["Enemy"];


        Enemies = new Entity[positions.Length];

        for (int i = 0; i < Enemies.Length; i++)
        {
            GameObject entityGO = pool.GetObjectForType("Entity", true, positions[i]);
            if (entityGO == null)
            {
                // Make a new one?
                return;
            }

            EntityComponent[] components = new EntityComponent[]
            {
                new RenderComponent("Player"),
                new PositionComponent(positions[i].x, positions[i].y),
                new FighterComponent(10, 10),
                new EnemyComponent()
            };

            Entity newEntity = new Entity(proto.Name, proto.entityType, components);

            newEntity.InitComponent(entityGO);

            EntityGOMap.Add(newEntity, entityGO);
            int index = i;
            // Register end turn CB for enemy
            newEntity.CanEndTurnCB = () => CanEnemyEndTurn(index);
            Enemies[i] = newEntity;
        }

        
    }
    public bool CanEnemyEndTurn(int enemyIndex)
    {
        if (enemyIndex >= Enemies.Length - 1)
            return true;
        // check if any rest of the enemies in the array are active
        for (int i = enemyIndex + 1; i < Enemies.Length; i++)
        {
            if (Enemies[i].isActive)
            {
                return false;
            }
        }
        return true;
    }
    public bool CanPlayerEndTurn()
    {
        return true;
    }
}
