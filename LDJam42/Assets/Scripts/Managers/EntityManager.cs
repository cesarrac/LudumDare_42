using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Global;

public class EntityManager : MonoBehaviour
{
    public static EntityManager instance { get; protected set; }
    Dictionary<string, EntityPrototype> EntityProtoMap;
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
        EntityProtoMap = new Dictionary<string, EntityPrototype>();

        CreatePrototypes();

        Global.EntityDeath.RegisterListener(OnEntityDeath);
    }
    public void StopPlayer()
    {
        Player.OnActiveChange(false);
    }
    public void ClearItems()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            PoolEntity(Items[i]);
            Items[i].OnActiveChange(false);
        }
        Items.Clear();
    }

    public void ClearEnemies()
    {
        for (int i = 0; i < Enemies.Length; i++)
        {
            PoolEntity(Enemies[i]);
            Enemies[i].OnActiveChange(false);
        }
        Enemies = null;
    }

    private void OnEntityDeath(EntityDeath data)
    {
        if (data.deadEntity.entityType != EntityType.Unit)
        {
            // do Items die?
            return;
        }
        if (data.deadEntity.isPlayer)
        {
            // Player dead! Time to roll up a new character
            Global.PlayerDeath playerDeath = new PlayerDeath();
            playerDeath.playerName = data.deadEntity.Name;
            playerDeath.points = 100; // TODO get this from total artifacts collected
            playerDeath.playerEntity = data.deadEntity;
            playerDeath.FireEvent();
            PoolEntity(data.deadEntity);
            return;
        }

        DeactivateEnemy(data.deadEntity);
    }

    private void DeactivateEnemy(Entity deadEntity)
    {
        foreach(Entity enemy in Enemies)
        {
            if (enemy == deadEntity)
            {
                enemy.OnActiveChange(false);
                break;
            }
        }
        PoolEntity(deadEntity);
    }

    private void PoolEntity(Entity entity)
    {
        if (EntityGOMap.ContainsKey(entity) == false)
            return;
        EntityGOMap[entity].name = "Entity";
        pool.PoolObject(EntityGOMap[entity]);
        EntityGOMap.Remove(entity);
    }

    private void CreatePrototypes()
    {
        EntityPrototype playerProto = new EntityPrototype("Player", EntityType.Unit,
                                                          new ComponentBlueprint[]
                                                          {
                                                              new ComponentBlueprint("RenderComponent",
                                                                                    new ComponentParam[]{
                                                                                        new ComponentParam(FieldType.STRING, "Player")
                                                                                    }),
                                                              new ComponentBlueprint("PositionComponent",
                                                                                    new ComponentParam[]{
                                                                                        new ComponentParam(FieldType.FLOAT, "0"),
                                                                                        new ComponentParam(FieldType.FLOAT, "0")
                                                                                    }),
                                                              new ComponentBlueprint("FighterComponent",
                                                                                    new ComponentParam[]{
                                                                                        new ComponentParam(FieldType.INT, "2"),
                                                                                        new ComponentParam(FieldType.INT, "0"),
                                                                                        new ComponentParam(FieldType.FLOAT, "25")
                                                                                    }),
                                                              new ComponentBlueprint("AbilityComponent",
                                                                                    new ComponentParam[]{})
                                                          });

        EntityProtoMap.Add("Player", playerProto);

        EntityPrototype enemyProto = new EntityPrototype("Enemy", EntityType.Unit,
                                                          new ComponentBlueprint[]
                                                          {
                                                              new ComponentBlueprint("RenderComponent",
                                                                                    new ComponentParam[]{
                                                                                        new ComponentParam(FieldType.STRING, "Enemy")
                                                                                    }),
                                                              new ComponentBlueprint("PositionComponent",
                                                                                    new ComponentParam[]{
                                                                                        new ComponentParam(FieldType.FLOAT, "0"),
                                                                                        new ComponentParam(FieldType.FLOAT, "0")
                                                                                    }),
                                                              new ComponentBlueprint("FighterComponent",
                                                                                    new ComponentParam[]{
                                                                                        new ComponentParam(FieldType.INT, "1"),
                                                                                        new ComponentParam(FieldType.INT, "0"),
                                                                                        new ComponentParam(FieldType.FLOAT, "1")
                                                                                    }),
                                                              new ComponentBlueprint("EnemyComponent",
                                                                                    new ComponentParam[]{
                                                                                    }),
                                                          });
        EntityProtoMap.Add("Enemy", enemyProto);
    }

    public void SpawnPlayer(Vector2 position)
    {
        if (pool == null)
        {
            pool = ObjectPool.instance;
        }

        if (Player != null)
        {
            PositionComponent posComp = (PositionComponent)Player.GetEntityComponent(ComponentID.Position);
            posComp.moveData = new MoveData(position.x, position.y);
            EntityGOMap[Player].transform.position = position;
            EntityActionManager.instance.InitEntityOnTile(Player, posComp.moveData);
            return;
        }

        EntityPrototype proto = EntityProtoMap["Player"];
        EntityComponent[] components = ReadProtoComponents(proto.components);

        GameObject entityGO = pool.GetObjectForType("Entity", true, position);
        if (entityGO == null)
        {
            // Make a new one?
            return;
        }

        Entity newEntity = new Entity(proto.Name, proto.entityType, components, isPlayer: true);


        newEntity.InitComponent(entityGO);

        EntityGOMap.Add(newEntity, entityGO);

        newEntity.CanEndTurnCB = CanPlayerEndTurn;

        AbilityComponent abilityComponent = (AbilityComponent)newEntity.GetEntityComponent(ComponentID.Abilities);
        abilityComponent.AddAbility(AbilityID.Blood_For_Light, true, "Remove Darkness for 25% HP");
        PositionComponent posC = (PositionComponent)newEntity.GetEntityComponent(ComponentID.Position);
        posC.moveData = new MoveData(position.x, position.y);

        // Register input callbacks
        PlayerInputSystem.instance.RegisterOnMoveInputCB(posC.Move);

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

        EntityPrototype proto = EntityProtoMap["Enemy"];


        Enemies = new Entity[positions.Length];

        for (int i = 0; i < Enemies.Length; i++)
        {
            GameObject entityGO = pool.GetObjectForType("Entity", true, positions[i]);
            if (entityGO == null)
            {
                // Make a new one?
                return;
            }
            EntityComponent[] components = ReadProtoComponents(proto.components);
            Entity newEntity = new Entity(proto.Name, proto.entityType, components);

            newEntity.InitComponent(entityGO);
            PositionComponent posC = (PositionComponent)newEntity.GetEntityComponent(ComponentID.Position);
            posC.moveData = new MoveData(positions[i].x, positions[i].y);

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
    public EntityComponent[] ReadProtoComponents(ComponentBlueprint[] protoComponents)
    {
        EntityComponent[] systems = new EntityComponent[protoComponents.Length];
        for (int x = 0; x < protoComponents.Length; x++)
        {
            object[] curParameters;
            //------------- EXCEPTION FOR READING ABILITIES:------------------------------------------------//
            //if (blueprint[x].className == "AbilitySystem")
            //{
            //    // Filled Param [0] is class name and [1] is description, so Ability parameters are >= [2]
            //    curParameters = new object[blueprint[x].compParams.Length - 2];

            //    string className = blueprint[x].ParamsAsStrings[0].Value;
            //    string desc = blueprint[x].ParamsAsStrings[1].Value;

            //    for (int i = 0; i < blueprint[x].Parameters.Length; i++)
            //    {
            //        if (blueprint[x].ParamsAsStrings[i + 2].FieldType == FieldType.STRING)
            //        {
            //            blueprint[x].Parameters[i] = blueprint[x].ParamsAsStrings[i + 2].Value;
            //        }
            //        else if (blueprint[x].ParamsAsStrings[i + 2].FieldType == FieldType.INT)
            //        {
            //            int param = 0;
            //            if (Int32.TryParse(blueprint[x].ParamsAsStrings[i + 2].Value, out param) == false)
            //                continue;
            //            blueprint[x].Parameters[i] = param;
            //        }
            //        else if (blueprint[x].ParamsAsStrings[i + 2].FieldType == FieldType.FLOAT)
            //        {
            //            float param = 0;
            //            if (float.TryParse(blueprint[x].ParamsAsStrings[i + 2].Value, out param) == false)
            //                continue;
            //            blueprint[x].Parameters[i] = param;
            //        }

            //    }

            //    systems[x] = new AbilitySystem(className, ReadAbility(className, blueprint[x].Parameters), desc);
            //    continue;
            //}   
            // --------------------------------------------------------------------------------------------------

            // make params an array of object
            curParameters = new object[protoComponents[x].compParams.Length];

            // Loop through parameters and try filling them
            for (int i = 0; i < protoComponents[x].compParams.Length; i++)
            {
                if (protoComponents[x].compParams[i].fieldType == FieldType.STRING)
                {
                    curParameters[i] = protoComponents[x].compParams[i].value;
                }
                else if (protoComponents[x].compParams[i].fieldType == FieldType.INT)
                {
                    int param = 0;
                    if (Int32.TryParse(protoComponents[x].compParams[i].value, out param) == false)
                        continue;
                    curParameters[i] = param;
                }
                else if (protoComponents[x].compParams[i].fieldType == FieldType.FLOAT)
                {
                    float param = 0;
                    if (float.TryParse(protoComponents[x].compParams[i].value, out param) == false)
                        continue;
                    curParameters[i] = param;
                }
            }
            var type = Type.GetType(protoComponents[x].className);
            object o = Activator.CreateInstance(type, curParameters);


            systems[x] = (EntityComponent)o;
        }
        return systems;
    }
}


public struct EntityPrototype
{
    public ComponentBlueprint[] components;
    public string Name;
    public EntityType entityType;

    public EntityPrototype(string name, EntityType entityType, ComponentBlueprint[] components)
    {
        this.components = components;
        Name = name;
        this.entityType = entityType;
    }
}
public struct ComponentBlueprint
{
    public string className;
    public ComponentParam[] compParams;

    public ComponentBlueprint(string className, ComponentParam[] compParams)
    {
        this.className = className;
        this.compParams = compParams;
    }
}
public struct ComponentParam
{
    public FieldType fieldType;
    public string value;

    public ComponentParam(FieldType fieldType, string value)
    {
        this.fieldType = fieldType;
        this.value = value;
    }
}
public enum FieldType
{
    STRING, INT, FLOAT
}