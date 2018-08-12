using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentComponent : EntityComponent
{
    public WeaponComponent weapon { get; protected set; }
    public ArmorComponent armor { get; protected set; }
    PositionComponent thisPosition;

    public EquipmentComponent() : base(ComponentID.Equipment)
    {
    }

    public override void Init(Entity entity, GameObject entityGO)
    {
        thisPosition = (PositionComponent)entity.GetEntityComponent(ComponentID.Position);
    }

    public bool AddEquipment(ItemComponent itemComponent)
    {
        Entity itemEntity = itemComponent.thisEntity;
        if (itemComponent.itemType == ItemType.Weapon)
        {
            EntityComponent wComp = itemEntity.GetEntityComponent(ComponentID.Weapon);
            if (wComp == null)
                return false;
            if (weapon != null)
                DropCurWpn();
            weapon = (WeaponComponent)wComp;
            Debug.Log("WEAPON added to equipment - " + itemComponent.itemName);
            return true;
        }
        else if (itemComponent.itemType == ItemType.Armor)
        {
            EntityComponent aComp = itemEntity.GetEntityComponent(ComponentID.Armor);
            if (aComp == null)
                return false;
            if (armor != null)
                DropCurArmor();
            armor = (ArmorComponent)aComp;
            Debug.Log("ARMOR added to equipment - " + itemComponent.itemName);
            return true;
        }
        return false;
    }
    void DropCurWpn()
    {
        weapon.Drop(thisPosition.moveData);
    }
    void DropCurArmor()
    {
        armor.Drop(thisPosition.moveData);
    }
    public int GetAttackPower()
    {
        int power = 0;
        if (weapon != null)
            power += weapon.weaponAttackStats.AttackPower;
        if (armor != null)
            power += armor.armorAttackStats.AttackPower;

        return power;
    }
    public int GetDefensePower()
    {
        int power = 0;
        if (weapon != null)
            power += weapon.weaponAttackStats.DefensePower;
        if (armor != null)
            power += armor.armorAttackStats.DefensePower;

        return power;
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
