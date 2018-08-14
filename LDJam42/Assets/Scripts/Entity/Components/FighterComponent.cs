using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterComponent : EntityComponent
{
    AttackData attackData;
    public Entity thisEntity { get; protected set; }
    float maxHP;
    public float curHP { get; protected set; }
    public EquipmentComponent equipment { get; protected set; }
    Action<float, float> OnHPChanged;

    int[] hpAtLevel = new int[] { 60,80,100,120,140,160,180,200,220,240 };
    int hpIndex = 0;

    public FighterComponent(int attackPower, int defensePower, float startHP) : base(ComponentID.Fighter)
    {
        attackData = new AttackData(attackPower, defensePower);
        this.maxHP = curHP = startHP;
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
            UI_Manager.instance.HandlePlayerHealthUI(curHP, maxHP);
        }
        else
        {
            OnHPChanged += UI_Manager.instance.HandleEnemyHealthUI;
            UI_Manager.instance.HandleEnemyHealthUI(curHP, maxHP);
        }
    }
    
    public int GetAttackPower()
    {
        if (equipment != null)
        {
            return equipment.GetAttackPower() + attackData.AttackPower;
        }
        return attackData.AttackPower;
    }

    public void GainHealth(int healthGained)
    {
        curHP += healthGained;
        curHP = Mathf.Clamp(curHP, 0, maxHP);
    }
    public void IncreaseMaxHealth()
    {
        if (hpIndex >= hpAtLevel.Length)
        {
            return;
        }
        hpIndex++;
        maxHP = hpAtLevel[hpIndex];
        UI_Manager.instance.HandlePlayerHealthUI(curHP, maxHP);
    }

    public int GetDefensePower()
    {
        if (equipment != null)
        {
            return equipment.GetDefensePower() + attackData.DefensePower;
        }
        return attackData.DefensePower;
    }
    public bool ReceiveDamage(float damage, bool byPoison = false)
    {
        // todo add damage received call back to resist/mitigate any damage
        curHP -= damage;
        curHP = Mathf.Clamp(curHP, 0, 1000);

        if (OnHPChanged != null)
        {
            OnHPChanged(curHP, maxHP);
        }
        if (curHP <= 0)
        {
            if (byPoison == true)
            {
                if (thisEntity.isPlayer == true)
                {
                    MessageLog_Manager.NewMessage("The POISON CONSUMES YOU!", Color.red);

                }
                else
                {
                    MessageLog_Manager.NewMessage("The POISON CONSUMES " + thisEntity.Name, Color.red);

                }
            }
            
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
