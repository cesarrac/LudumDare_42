using UnityEngine;
using System.Collections;
using System;

public class EntityActionManager : MonoBehaviour
{
    public static EntityActionManager instance { get; protected set; }
    MapManager mapManager;
    CameraShaker cameraShaker;
    private void Awake()
    {
        instance = this;
    }
    public void Init()
    {
        mapManager = MapManager.instance;
        cameraShaker = CameraShaker.instance;
    }
    public void EntityOnTileChanged(Entity entity, MoveData curPositionData)
    {
        MapTile curTile = mapManager.Map.GetTile(new Vector2(curPositionData.X, curPositionData.Y));
        if (curTile == null)
        {
            Debug.LogError("Entity: " + entity.Name + " is standing on a NULL TILE!!");
            return;
        }
        if (entity.isActive == true)
            curTile.RegisterEntity(entity);
        else
            curTile.UnRegisterEntity(entity);
    }
    /// <summary>
    /// Returns true if Entity can move to next tile,
    /// if false it will call the correct action to do
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="lastPositionData"></param>
    /// <param name="newPositionData"></param>
    /// <returns></returns>
    public bool DoTileAction(Entity entity, MoveData lastPositionData, MoveData newPositionData)
    {
        MapTile curTile = mapManager.Map.GetTile(new Vector2(lastPositionData.X, lastPositionData.Y));
        if (curTile == null)
        {
            Debug.LogError("Entity: " + entity.Name + " is standing on a NULL TILE!!");
            return false;
        }
        MapTile nextTile = mapManager.Map.GetTile(new Vector2(newPositionData.X, newPositionData.Y));
        if (nextTile == null)
        {
            Debug.LogError("Entity: " + entity.Name + " is moving to a NULL TILE!!");
            return false; 
        }

        if (nextTile.entities.Count > 0)
        {
            if (InteractTileEntities(entity, nextTile) == false)
            {
                // check if this can end turn
                //if (entity.CanEndTurnCB != null)
                //{
                //    if (entity.CanEndTurnCB() == true)
                //    {
                //        TurnManager.instance.FinishTurn();
                //    }
                //}

                return false;
            }
        }

        // DARKNESS DAMAGE
        if (curTile.tileType == TileType.Darkness)
        {
            if (entity.isPlayer == true)
            {
                MessageLog_Manager.NewMessage("The POISON CONSUMES YOU!", Color.red);

            }
            else
            {
                MessageLog_Manager.NewMessage("The POISON CONSUMES " + entity.Name, Color.red);
            }
            FighterComponent attacker = (FighterComponent)entity.GetEntityComponent(ComponentID.Fighter);
            attacker.ReceiveDamage(1000);
            return false;
        }

        // Normal move action happens
        if (nextTile != curTile)
        {
            curTile.UnRegisterEntity(entity);
            nextTile.RegisterEntity(entity);
            if (entity.isPlayer == true && nextTile.tileType == TileType.Exit)
            {
                Global.PlayerReachedExit playerReachedExit = new Global.PlayerReachedExit();
                playerReachedExit.exitPosition = nextTile.WorldPosition;
                playerReachedExit.FireEvent();
                return false;
            }
        }

        // check if this can end turn
        //if (entity.CanEndTurnCB != null)
        //{
        //    if (entity.CanEndTurnCB() == true)
        //    {
        //        TurnManager.instance.FinishTurn();
        //    }
        //}
        return true;
    }

    /// <summary>
    /// Interact with entity on tile
    /// Returns false if it blocks the entity actor's movement
    /// </summary>
    /// <param name="interactor"></param>
    /// <param name="nextTile"></param>
    /// <returns></returns>
    private bool InteractTileEntities(Entity interactor, MapTile nextTile)
    {
        if (nextTile.entities.Count > 1)
        {
            // figure this out...
            Debug.LogError("tried to step into a tile that has more than one entity registered on it!");
            return false;
        }
        if (nextTile.entities[0].entityType == EntityType.Unit)
        {
            if (nextTile.entities[0].faction == interactor.faction)
                return false;
            FighterComponent attacker = (FighterComponent)interactor.GetEntityComponent(ComponentID.Fighter);
            FighterComponent defender = (FighterComponent)nextTile.entities[0].GetEntityComponent(ComponentID.Fighter);
            return DoCombat(attacker, defender);
        }
        else if (nextTile.entities[0].entityType == EntityType.Item)
        {
            Debug.Log("ON AN ITEM TILE!!");
            // do item pick up
            EntityComponent comp = interactor.GetEntityComponent(ComponentID.Inventory);
            if (comp == null)
                return true;
            ItemComponent item = (ItemComponent)nextTile.entities[0].GetEntityComponent(ComponentID.Item);
            InventoryComponent inventory = (InventoryComponent)comp;
            if (inventory.AddItem(item) == true)
            {
                item.PickUp();
            }
            return true;
        }

        return true;
    }

    /// <summary>
    /// Handle entity combat.
    /// Return true if combat resulted in defender's death
    /// and attacker is allowed to move unto that tile.
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns></returns>
    private bool DoCombat(FighterComponent attacker, FighterComponent defender)
    {
        int attackPower = attacker.GetAttackPower();
       // Debug.Log("Attacker attacks with power " + attackPower);
        //Debug.Log("Defender defends with power " + defender.GetDefensePower());
        //if (attacker.thisEntity.isPlayer == true)
        //{
        //    MessageLog_Manager.NewMessage("You attack the " + defender.thisEntity.Name + " with " + attackPower + " attack!", Color.red);
        //}
        int damage = attackPower - defender.GetDefensePower();
        //Debug.Log("After defense mitigation... damage is " + damage);

        if (damage > 0)
        {
            if (defender.thisEntity.isPlayer == true)
            {
                cameraShaker.AddTrauma(5.2f, 2.8f);
                MessageLog_Manager.NewMessage(attacker.thisEntity.Name + " hits you for " + damage.ToString() + "!", Color.white);
            }
            else
            {
                MessageLog_Manager.NewMessage("You hit the " + defender.thisEntity.Name + " for " + damage.ToString() + "!", Color.red);
            }
        }
        else
        {
            if (attacker.thisEntity.isPlayer == true)
            {
                MessageLog_Manager.NewMessage(defender.thisEntity.Name + "'s defense absorb your attack!", Color.white);
            }
            else
            {
                MessageLog_Manager.NewMessage("Your defenses absorb the attack!", Color.white);
            }
        }
        
        bool result = defender.ReceiveDamage(damage);
        if (result == true)
        {
            if (attacker.thisEntity.isPlayer == true)
            {
                MessageLog_Manager.NewMessage(defender.thisEntity.Name + " DIES!", Color.red);
            }
            else
                MessageLog_Manager.NewMessage(attacker.thisEntity.Name + " KILLS YOU!", Color.red);
        }

        return result;
    }
}
