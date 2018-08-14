using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionComponent : EntityComponent
{
    Action<MoveData> OnInputProcessed;
    Func<PositionComponent, Entity, bool> OnInputNeeded;

    public MoveData moveData, directionData;

    Func<MoveData, bool> CanMoveTo;

    Entity thisEntity;

    EntityActionManager actionManager;

    public PositionComponent(float x, float y) : base(ComponentID.Position)
    {
        moveData = new MoveData(x, y);
    }

    public override void Init(Entity entity, GameObject entityGO)
    {
        if (entityGO == null)
            return;

        OnInputNeeded = AbilitySystem.instance.IsInputNeeded;
        RegisterCBListener<Action<MoveData>>((data) => MoveSystem.instance.ChangePosition(data, entityGO.transform));
        CanMoveTo = MapManager.instance.CanMoveTo;
        thisEntity = entity;
        actionManager = EntityActionManager.instance;
        actionManager.EntityOnTileChanged(thisEntity, moveData);
        //thisEntity.OnActiveChanged += ClearCB;
    }
    void ClearCB(bool isActive)
    {
        if (isActive == true)
            return;
        OnInputNeeded = null;
        OnInputProcessed = null;
        CanMoveTo = null;
    }

    public void Move(MoveData direction, bool abilityMove = false)
    {
        if (direction.X == 0 && direction.Y == 0)
        {
            TryEndTurn();
            return;
        }

        directionData = direction;

        if (OnInputNeeded != null && abilityMove == false)
        {
            if (OnInputNeeded(this, thisEntity) == true)
                return;
        }

        MoveData newMoveData = new MoveData(this.moveData.X + direction.X, this.moveData.Y + direction.Y);
        // Save current move data to unregister from current tile
        MoveData lastData = moveData;
        if (CanMoveTo(newMoveData) == false)
        {
            if (thisEntity.isPlayer == false)
                TryEndTurn();
            return;
        }
        if (actionManager.DoTileAction(thisEntity, lastData, newMoveData) == false)
        {
            if (thisEntity.isActive == true && thisEntity.CanEndTurnCB() == true)
            {
                TurnManager.instance.FinishTurn();
            }

            return;
        }

        // Set new move data
        moveData = newMoveData;
       
        // Move transform
        if (OnInputProcessed != null)
        {
            OnInputProcessed(moveData);
        }

        if (thisEntity.CanEndTurnCB() == true)
        {
            TurnManager.instance.FinishTurn();
        }
        //Debug.Log("new position x:" + moveData.X + " new position y :" + moveData.Y);
    }

    void TryEndTurn()
    {
        if (thisEntity.CanEndTurnCB != null)
        {
            if (thisEntity.CanEndTurnCB() == true)
            {
                TurnManager.instance.FinishTurn();
            }
        }
    }

    public MoveData GetPositionData()
    {
        return moveData;
    }

    public override void RegisterCBListener<T>(T listener)
    {
        OnInputProcessed += listener as Action<MoveData>;
    }

    public override void UnRegisterCBListener<T>(T listener)
    {
        OnInputProcessed -= listener as Action<MoveData>;
    }
}
