using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Global;

public class EntityManager : MonoBehaviour
{
    public static EntityManager instance { get; protected set; }
    //Dictionary<string, EntityPrototype> EntityProtoMap;
    public EntityPrototype[] prototypes;
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
       // EntityProtoMap = new Dictionary<string, EntityPrototype>();

        CreatePrototypes();

        Global.EntityDeath.RegisterListener(OnEntityDeath);
    }
    public void StopPlayer()
    {
        if (Player == null)
            return;
        Player.ChangeActiveStatus(false);
    }
    //public void ClearPlayer()
    //{
    //    PositionComponent posC = (PositionComponent)Player.GetEntityComponent(ComponentID.Position);
    //    PlayerInputSystem.instance.UnRegisterOnMoveInputCB(posC.Move);
    //    Player = null;
    //}
    public void ClearItems()
    {
        List<Entity> itemsToRemove = new List<Entity>();
        for (int i = 0; i < Items.Count; i++)
        {
            // IF the item is in the player's inventory
            // we don't want it to pool...
            // Item's in inventory will already be isActive = false
            // so avoid adding it to the remove list by checking isActive
            if (Items[i].isActive == false)
                continue;
            itemsToRemove.Add(Items[i]);
        }
        for (int x = 0; x < itemsToRemove.Count; x++)
        {
            itemsToRemove[x].ChangeActiveStatus(false);
            PoolEntity(itemsToRemove[x]);
            Items.Remove(itemsToRemove[x]);
        }
    }

    public void ClearEnemies()
    {
        if (Enemies == null)
            return;
        for (int i = 0; i < Enemies.Length; i++)
        {
            PoolEntity(Enemies[i]);
            Enemies[i].ChangeActiveStatus(false);
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
            Player.ChangeActiveStatus(false);
            PoolEntity(data.deadEntity);
            return;
        }
        if (data.deadEntity.CanEndTurnCB != null)
        {
            if (data.deadEntity.CanEndTurnCB() == true)
            {
                TurnManager.instance.FinishTurn();
            }
        }
        DeactivateEnemy(data.deadEntity);
    }

    private void DeactivateEnemy(Entity deadEntity)
    {
        foreach(Entity enemy in Enemies)
        {
            if (enemy == deadEntity)
            {
                enemy.ChangeActiveStatus(false);
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
        //EntityPrototype playerProto = new EntityPrototype("Player", EntityType.Unit,
        //                                                  new ComponentBlueprint[]
        //                                                  {
        //                                                      new ComponentBlueprint("RenderComponent",
        //                                                                            new ComponentParam[]{
        //                                                                                new ComponentParam(FieldType.STRING, "Player")
        //                                                                            }),
        //                                                      new ComponentBlueprint("PositionComponent",
        //                                                                            new ComponentParam[]{
        //                                                                                new ComponentParam(FieldType.FLOAT, "0"),
        //                                                                                new ComponentParam(FieldType.FLOAT, "0")
        //                                                                            }),
        //                                                      new ComponentBlueprint("FighterComponent",
        //                                                                            new ComponentParam[]{
        //                                                                                new ComponentParam(FieldType.INT, "2"),
        //                                                                                new ComponentParam(FieldType.INT, "0"),
        //                                                                                new ComponentParam(FieldType.FLOAT, "25")
        //                                                                            }),
        //                                                      new ComponentBlueprint("AbilityComponent",
        //                                                                            new ComponentParam[]{})
        //                                                  });

        //EntityProtoMap.Add("Player", playerProto);

        //EntityPrototype enemyProto = new EntityPrototype("Enemy", EntityType.Unit,
        //                                                  new ComponentBlueprint[]
        //                                                  {
        //                                                      new ComponentBlueprint("RenderComponent",
        //                                                                            new ComponentParam[]{
        //                                                                                new ComponentParam(FieldType.STRING, "Enemy")
        //                                                                            }),
        //                                                      new ComponentBlueprint("PositionComponent",
        //                                                                            new ComponentParam[]{
        //                                                                                new ComponentParam(FieldType.FLOAT, "0"),
        //                                                                                new ComponentParam(FieldType.FLOAT, "0")
        //                                                                            }),
        //                                                      new ComponentBlueprint("FighterComponent",
        //                                                                            new ComponentParam[]{
        //                                                                                new ComponentParam(FieldType.INT, "1"),
        //                                                                                new ComponentParam(FieldType.INT, "0"),
        //                                                                                new ComponentParam(FieldType.FLOAT, "1")
        //                                                                            }),
        //                                                      new ComponentBlueprint("EnemyComponent",
        //                                                                            new ComponentParam[]{
        //                                                                            }),
        //                                                  });
        //EntityProtoMap.Add("Enemy", enemyProto);
    }

    private EntityPrototype GetProtoType(string entityName)
    {
        for (int i = 0; i < prototypes.Length; i++)
        {
            if (prototypes[i].Name == entityName)
            {
                return prototypes[i];
            }
        }
        return null;
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
            EntityActionManager.instance.EntityOnTileChanged(Player, posComp.moveData);
            Player.ChangeActiveStatus(true);
            return;
        }

        EntityPrototype proto = GetProtoType("Player");//EntityProtoMap["Player"];
        EntityComponent[] components = ReadProtoComponents(proto.components);

        GameObject entityGO = pool.GetObjectForType("Player", true, position);
        if (entityGO == null)
        {
            // Make a new one?
            return;
        }

        Player = new Entity(proto.Name, proto.entityType, components, isPlayer: true);

        PositionComponent posC = (PositionComponent)Player.GetEntityComponent(ComponentID.Position);
        posC.moveData = new MoveData(position.x, position.y);

        Player.InitComponent(entityGO);
        
        AbilityComponent abilityComponent = (AbilityComponent)Player.GetEntityComponent(ComponentID.Abilities);
        abilityComponent.AddAbility(AbilityID.Blood_For_Light, true, "25% HP");

        //InventoryComponent inventoryComponent = (InventoryComponent)Player.GetEntityComponent(ComponentID.Inventory);
        //PlayerInputSystem.instance.AddDynamicKeys(() => inventoryComponent.Drop(0), "t");

        Player.ChangeActiveStatus(true);
    

        EntityGOMap.Add(Player, entityGO);

        Player.CanEndTurnCB = CanPlayerEndTurn;

        // Register input callbacks
        PlayerInputSystem.instance.RegisterOnMoveInputCB((data) => posC.Move(data));

        GetPlayerPositionData += posC.GetPositionData;

    }
    public bool CanPlayerEndTurn()
    {
        return true;
    }


    public void SpawnEnemies(string enemyPrototypeName, Vector2[] positions)
    {
        EntityPrototype proto = GetProtoType(enemyPrototypeName); //EntityProtoMap["Enemy"];
        if (proto == null)
        {
            Debug.LogError("EntityManager could not find a protoype called " + enemyPrototypeName + " and cannot spawn it!");
            return;
        }
        int enemyIndex = 0;
        if (Enemies == null || Enemies.Length <= 0)
        {
            Enemies = new Entity[positions.Length];
            Debug.Log("Spawning " + Enemies.Length + " enemies");
        }
        else
        {
            // extend enemies
            Entity[] origEnemies = Enemies;
            Enemies = new Entity[origEnemies.Length + positions.Length];
            for (int i = 0; i < origEnemies.Length; i++)
            {
                Enemies[i] = origEnemies[i];
            }
            enemyIndex = origEnemies.Length;
        }

        int posIndex = 0;
        for (int i = enemyIndex; i < Enemies.Length; i++)
        {
            GameObject entityGO = pool.GetObjectForType("Entity", true, positions[posIndex]);
            if (entityGO == null)
            {
                // Make a new one?
                return;
            }
            EntityComponent[] components = ReadProtoComponents(proto.components);
            Entity newEntity = new Entity(proto.Name, proto.entityType, components);

            PositionComponent posC = (PositionComponent)newEntity.GetEntityComponent(ComponentID.Position);
            posC.moveData = new MoveData(positions[posIndex].x, positions[posIndex].y);

            newEntity.InitComponent(entityGO);

            EntityGOMap.Add(newEntity, entityGO);
            int index = i;
            // Register end turn CB for enemy
            newEntity.CanEndTurnCB = () => CanEnemyEndTurn(index);
            Enemies[i] = newEntity;

            posIndex++;
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
   
    public void SpawnItem(string itemPrototypeName, Vector2 position)
    {
        EntityPrototype prototype = GetProtoType(itemPrototypeName);
        if (prototype == null)
        {
            Debug.LogError("Could no find item prototype named " + itemPrototypeName);
            return;
        }
        GameObject entityGO = pool.GetObjectForType("Entity", true, position);
        if (entityGO == null)
        {
            // Make a new one?
            return;
        }
        Entity item = new Entity(itemPrototypeName, prototype.entityType, ReadProtoComponents(prototype.components));
       
        ItemComponent itemC = (ItemComponent)item.GetEntityComponent(ComponentID.Item);
        itemC.positionData = new MoveData(position.x, position.y);
        item.InitComponent(entityGO);
        EntityGOMap.Add(item, entityGO);
        Items.Add(item);
    }
    /// <summary>
    /// Call when an entity has been created but
    /// it has no gameobject ( items )
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="position"></param>
    public GameObject SpawnEntityGO(Entity entity, MoveData position, bool initsComponents = false)
    {
        // Make sure that in fact we don't have this entity's gameobj
        if (EntityGOMap.ContainsKey(entity) == true)
            return null;
        GameObject gobj = pool.GetObjectForType("Entity", true, new Vector2(position.X, position.Y));
        if (gobj == null)
        {
            // Make a new one?
            return null;
        }
        if (initsComponents == true)
            entity.InitComponent(gobj);

        return gobj;
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


