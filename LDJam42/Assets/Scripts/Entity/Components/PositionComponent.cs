using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionComponent : EntityComponent
{
    Action<MoveData> OnInputReceived;

    public MoveData moveData;

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
        RegisterCBListener<Action<MoveData>>((data) => MoveSystem.instance.ChangePosition(data, entityGO.transform));
        CanMoveTo += MapManager.instance.CanMoveTo;
        thisEntity = entity;
        actionManager = EntityActionManager.instance;
        actionManager.InitEntityOnTile(thisEntity, moveData);
    }

    public void Move(MoveData directionData)
    {
        MoveData newMoveData = new MoveData(this.moveData.X + directionData.X, this.moveData.Y + directionData.Y);
        // Save current move data to unregister from current tile
        MoveData lastData = moveData;

        if (CanMoveTo(newMoveData) == false)
            return;
        if (actionManager.DoTileAction(thisEntity, lastData, newMoveData) == false)
            return;

        // Set new move data
        moveData = newMoveData;
       
        // Move transform
        if (OnInputReceived != null)
        {
            OnInputReceived(moveData);
        }

        //Debug.Log("new position x:" + moveData.X + " new position y :" + moveData.Y);
    }

    public MoveData GetPositionData()
    {
        return moveData;
    }

    public override void RegisterCBListener<T>(T listener)
    {
        OnInputReceived += listener as Action<MoveData>;
    }

    public override void UnRegisterCBListener<T>(T listener)
    {
        OnInputReceived -= listener as Action<MoveData>;
    }
}
