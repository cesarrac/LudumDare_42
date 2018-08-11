using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterComponent : EntityComponent
{
    AttackData attackData;
    Entity thisEntity;
    float startHP;
    public float curHP;

    public FighterComponent(int attackPower, int defensePower, float startHP) : base(ComponentID.Fighter)
    {
        attackData = new AttackData(attackPower, defensePower);
        this.startHP = curHP = startHP;
    }

    public override void Init(Entity entity, GameObject entityGO)
    {
        thisEntity = entity;
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
    public bool ReceiveDamage(int damage)
    {
        // todo add damage received call back to resist/mitigate any damage
        curHP -= damage;
        curHP = Mathf.Clamp(curHP, 0, 1000);

        if (curHP <= 0)
        {
            Global.EntityDeath death = new Global.EntityDeath();
            death.deadEntity = thisEntity;
            death.FireEvent();
            return true;
        }
        return false;
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
