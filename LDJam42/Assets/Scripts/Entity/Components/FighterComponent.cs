using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterComponent : EntityComponent
{
    AttackData attackData;
    public Entity thisEntity { get; protected set; }
    float startHP;
    public float curHP { get; protected set; }
    public EquipmentComponent equipment { get; protected set; }
    Action<float, float> OnHPChanged; 

    public FighterComponent(int attackPower, int defensePower, float startHP) : base(ComponentID.Fighter)
    {
        attackData = new AttackData(attackPower, defensePower);
        this.startHP = curHP = startHP;
    }

    public override void Init(Entity entity, GameObject entityGO)
    {
        thisEntity = entity;
        EntityComponent comp = entity.GetEntityComponent(ComponentID.Equipment);
        if (comp != null)
            equipment = (EquipmentComponent)comp;

        if (thisEntity.isPlayer == true)
        {
            OnHPChanged += UI_Manager.instance.HandlePlayerHealthUI;
            UI_Manager.instance.HandlePlayerHealthUI(curHP, startHP);
        }
        else
        {
            OnHPChanged += UI_Manager.instance.HandleEnemyHealthUI;
            UI_Manager.instance.HandleEnemyHealthUI(curHP, startHP);
        }
    }
    
    public int GetAttackPower()
    {
        if (equipment != null)
        {
            int power = equipment.GetAttackPower();
            if (power > 0)
                return power;
        }
        return attackData.AttackPower;
    }

    public void GainHealth(int healthGained)
    {
        curHP += healthGained;
        curHP = Mathf.Clamp(curHP, 0, startHP);
    }

    public int GetDefensePower()
    {
        if (equipment != null)
        {
            int defnse = equipment.GetDefensePower();
            if (defnse > 0)
                return defnse;
        }
        return attackData.DefensePower;
    }
    public bool ReceiveDamage(float damage)
    {
        // todo add damage received call back to resist/mitigate any damage
        curHP -= damage;
        curHP = Mathf.Clamp(curHP, 0, 1000);

        if (OnHPChanged != null)
        {
            OnHPChanged(curHP, startHP);
        }
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
