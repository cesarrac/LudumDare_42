using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterComponent : EntityComponent
{
    AttackData attackData;

    public FighterComponent(int attackPower, int defensePower) : base(ComponentID.Fighter)
    {
        attackData = new AttackData(attackPower, defensePower);
    }

    public override void Init(Entity entity, GameObject entityGO)
    {
        return;
    }

    public int GetAttackPower()
    {
        // todo add equipment call backs to add to attack power (weapon)
        return attackData.AttackPower;
    }
    public int GetDefensePower()
    {
        // todo add equipment call backs to add to defense power (armor)
        return attackData.DefensePower;
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

public struct AttackData
{
    private readonly int attackPower;
    private readonly int defensePower;

    public AttackData(int attackPower, int defensePower)
    {
        this.attackPower = attackPower;
        this.defensePower = defensePower;
    }

    public int AttackPower
    {
        get
        {
            return attackPower;
        }
    }

    public int DefensePower
    {
        get
        {
            return defensePower;
        }
    }
}
