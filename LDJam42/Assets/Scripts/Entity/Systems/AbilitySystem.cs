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
    public GameObject abilityInputWarning;

    private void Awake()
    {
        instance = this;
        casters = new List<AbilityCaster>();
        abilityInputWarning.SetActive(false);
    }
    public void CallAbility(Ability ability, Entity caster)
    {
        if (ability.NeedsInput == false)
        {
            CastAbility(caster, ability.ID, ability.Description);
        }
        else
        {
            abilityInputWarning.SetActive(true);
        }
        casters.Add(new AbilityCaster(caster, ability.ID, ability.NeedsInput, ability.Description));
    }

    private void CastAbility(Entity caster, AbilityID abilityID, string description)
    {
        PositionComponent poC = (PositionComponent)caster.GetEntityComponent(ComponentID.Position);
        FighterComponent fighter = (FighterComponent)caster.GetEntityComponent(ComponentID.Fighter);
        switch (abilityID)
        {
            case AbilityID.Blood_For_Light:
               
                CastBloodForLight(fighter, poC.moveData, poC.directionData, description);
                break;
            case AbilityID.Teleport:
                CastTeleport(fighter, poC, description);
                break;
            default:
                break;
        }
    }

    public bool IsInputNeeded(PositionComponent positionComponent, Entity entity)
    {
        int casterIndex = -1;
        for (int i = 0; i < casters.Count; i++)
        {
            if (casters[i].caster == entity && casters[i].waitsForInput == true)
            {
                CastAbility(entity, casters[i].abID, casters[i].abilityDesc);
                casterIndex = i;
                abilityInputWarning.SetActive(false);
                break;
            }
        }
        if (casterIndex >= 0)
        {
            casters.RemoveAt(casterIndex);
            return true;
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
    void CastTeleport(FighterComponent casterFighter, PositionComponent positionComponent, string abDesc)
    {
       

        Vector2 moveDir = new Vector2(positionComponent.directionData.X, positionComponent.directionData.Y) * 2;

        //Vector2 curDest = new Vector2(positionComponent.moveData.X, positionComponent.moveData.Y) + moveDir;
        //if (MapManager.instance.CanMoveTo(new MoveData(curDest.x, curDest.y)) == false){
        //    // try normal move
        //    if MapManager.instance.CanMoveTo(new MoveData(curDest.x, curDest.y)) == false)
        //    {
        //        return;
        //    }
        //}

        //positionComponent.moveData = new MoveData(positionComponent.moveData.)
 

        // Caster takes 40% of its health
        float dmg = casterFighter.curHP * 0.40f;
        casterFighter.ReceiveDamage(dmg);
        MessageLog_Manager.NewMessage(abDesc, Color.green);

        MoveData destData = new MoveData(moveDir.x, moveDir.y);
        positionComponent.Move(destData, true);

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
    Heal,
    Teleport
}
