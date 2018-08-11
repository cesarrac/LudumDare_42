using UnityEngine;
using System.Collections;
using System;

public class EntityActionManager : MonoBehaviour
{
    public static EntityActionManager instance { get; protected set; }
    MapManager mapManager;

    private void Awake()
    {
        instance = this;
    }
    public void Init()
    {
        mapManager = MapManager.instance;
    }
    public void InitEntityOnTile(Entity entity, MoveData curPositionData)
    {
        MapTile curTile = mapManager.Map.GetTile(new Vector2(curPositionData.X, curPositionData.Y));
        if (curTile == null)
        {
            Debug.LogError("Entity: " + entity.Name + " is standing on a NULL TILE!!");
            return;
        }
        curTile.RegisterEntity(entity);
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
                return false; 
        }

        // Normal move action happens
        if (nextTile != curTile)
        {
            curTile.UnRegisterEntity(entity);
            nextTile.RegisterEntity(entity);
        }

        // check if this can end turn
        if (entity.CanEndTurnCB != null)
        {
            if (entity.CanEndTurnCB() == true)
            {
                TurnManager.instance.FinishTurn();
            }
        }
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
        }
        if (nextTile.entities[0].entityType == EntityType.Unit)
        {
            FighterComponent attacker = (FighterComponent)interactor.GetEntityComponent(ComponentID.Fighter);
            FighterComponent defender = (FighterComponent)nextTile.entities[0].GetEntityComponent(ComponentID.Fighter);
            return DoCombat(attacker, defender);
        }
        else
        {

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
        Debug.Log("Attacker attacks with power " + attacker.GetAttackPower());
        Debug.Log("Defender defends with power " + defender.GetDefensePower());
        return false;
    }
}
