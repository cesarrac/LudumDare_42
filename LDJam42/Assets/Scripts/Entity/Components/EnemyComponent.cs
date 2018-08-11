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
        thisEntity.OnDeactivate = () => AiStateSystem.instance.UnRegisterAi(DoNextAction);
    }
    void DoNextAction()
    {
        //Debug.Log(thisEntity.Name + " does action");
        position.Move(new MoveData(1, 0));
        //actionManager.DoTileAction(thisEntity, position.moveData, new MoveData(position.moveData.X + 1, position.moveData.Y));
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
