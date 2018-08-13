using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class AbilitySystem : MonoBehaviour
{

    public static AbilitySystem instance { get; protected set; }

    bool callingAbility;
    MoveData inputDirection;

    List<AbilityCaster> casters;


    private void Awake()
    {
        instance = this;
        casters = new List<AbilityCaster>();
    }
    public void CallAbility(Ability ability, Entity caster)
    {
        if (ability.NeedsInput == false)
        {
            CastAbility(caster, ability.ID, ability.Description);
        }
        casters.Add(new AbilityCaster(caster, ability.ID, ability.NeedsInput, ability.Description));
    }

    private void CastAbility(Entity caster, AbilityID abilityID, string description)
    {
        switch (abilityID)
        {
            case AbilityID.Blood_For_Light:
                PositionComponent poC = (PositionComponent)caster.GetEntityComponent(ComponentID.Position);
                FighterComponent fighter = (FighterComponent)caster.GetEntityComponent(ComponentID.Fighter);
                CastBloodForLight(fighter, poC.moveData, poC.directionData, description);
                break;
            default:
                break;
        }
    }

    public bool IsInputNeeded(PositionComponent positionComponent, Entity entity)
    {
        for (int i = 0; i < casters.Count; i++)
        {
            if (casters[i].caster == entity && casters[i].waitsForInput == true)
            {
                CastAbility(entity, casters[i].abID, casters[i].abilityDesc);
                casters.RemoveAt(i);
                return true;
            }
        }
        
        return false;
        
    }

    void CastBloodForLight(FighterComponent casterFighter, MoveData position, MoveData direction, string abDesc)
    {
        if (MapManager.instance.ClearDarkTile(new Vector2(position.X + direction.X, position.Y + direction.Y)) == false)
            return;

        // Caster takes 25% of its health
        float dmg = casterFighter.curHP * 0.25f;
        casterFighter.ReceiveDamage(dmg);
        MessageLog_Manager.NewMessage(abDesc, Color.green);

    }

}

public struct AbilityCaster
{
    public Entity caster;
    public AbilityID abID;
    public bool waitsForInput;
    public string abilityDesc;

    public AbilityCaster(Entity caster, AbilityID abilityID, bool waitsForInput, string desc)
    {
        this.caster = caster;
        this.abID = abilityID;
        this.abilityDesc = desc;
        this.waitsForInput = waitsForInput;
    }
}

public enum AbilityID
{
    Blood_For_Light,
    Heal
}
