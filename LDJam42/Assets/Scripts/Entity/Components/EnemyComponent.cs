using UnityEngine;
using System.Collections;

public class EnemyComponent : EntityComponent
{
    Entity thisEntity;
    PositionComponent position;
    EntityActionManager actionManager;
    public EnemyComponent() : base(ComponentID.AI)
    {

    }

    public override void Init(Entity entity, GameObject entityGO)
    {
        thisEntity = entity;
        position = (PositionComponent)thisEntity.GetEntityComponent(ComponentID.Position);
        AiStateSystem.instance.RegisterAi(DoNextAction);
        actionManager = EntityActionManager.instance;
        thisEntity.OnActiveChanged += (active) => AiStateSystem.instance.UnRegisterAi(DoNextAction);
        thisEntity.OnActiveChanged += (active) => actionManager.EntityOnTileChanged(thisEntity, position.moveData);
    }
    void DoNextAction()
    {
        //Debug.Log(thisEntity.Name + " does action");

        // Figure out the direction this enemy needs to move to
        MoveData playerPosData = EntityManager.instance.GetPlayerPositionData();
        MoveData direction = AiStateSystem.instance.GetDirectionToTarget(position.moveData, playerPosData);
        position.Move(direction);
    }

    public override void RegisterCBListener<T>(T listener)
    {
        throw new System.NotImplementedException();
    }

    public override void UnRegisterCBListener<T>(T listener)
    {
        throw new System.NotImplementedException();
    }
}
