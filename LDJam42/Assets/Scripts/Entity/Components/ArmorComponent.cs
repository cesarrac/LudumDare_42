using UnityEngine;
using System.Collections;

public class ArmorComponent : EntityComponent
{
    ItemComponent itemComponent;
    public AttackData armorAttackStats { get; protected set; }

    public ArmorComponent(int attackPower, int defensePower) : base(ComponentID.Armor)
    {
        armorAttackStats = new AttackData(attackPower, defensePower);
    }

    public override void Init(Entity entity, GameObject entityGO)
    {
        itemComponent = (ItemComponent)entity.GetEntityComponent(ComponentID.Item);
    }
    public void Drop(MoveData positionData)
    {
        itemComponent.Drop(positionData);
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
