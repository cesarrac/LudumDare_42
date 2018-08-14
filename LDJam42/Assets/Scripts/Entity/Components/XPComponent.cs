using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPComponent : EntityComponent
{
    public XPData xpData { get; protected set; }
    FighterComponent fighter;
    AbilityComponent abilityComponent;

    public XPComponent() : base(ComponentID.XP)
    {
        OnXPChanged(new XPData(0, 1));
    }

    public override void Init(Entity entity, GameObject entityGO)
    {
       
        entity.OnActiveChanged += HandleCB;
        fighter = (FighterComponent)entity.GetEntityComponent(ComponentID.Fighter);
        abilityComponent = (AbilityComponent)entity.GetEntityComponent(ComponentID.Abilities);
    }

    void HandleCB(bool isActive)
    {
        if (isActive == true)
        {
            XPSystem.instance.RegisterGetXPCB(OnXPChanged);
            return;
        }
        XPSystem.instance.UnRegisterGetXPCB(OnXPChanged);
    }

    void OnXPChanged(XPData newXP)
    {
        if (xpData.TotalXP > 0)
        {
            int xpGain = newXP.TotalXP - xpData.TotalXP;
            MessageLog_Manager.NewMessage("You gain " + xpGain + " XP!", Color.yellow);

            if (newXP.CurXPLevel > xpData.CurXPLevel)
            {
                MessageLog_Manager.NewMessage("You are now level " + newXP.CurXPLevel, Color.yellow);
                // do level gain effect (gain max hp)
                fighter.IncreaseMaxHealth();

                if (xpData.CurXPLevel == 3)
                {
                    // gain teleport
                    abilityComponent.AddAbility(AbilityID.Teleport, true, "40% HP");
                }
            }
        }
        
        xpData = new XPData(newXP.TotalXP, newXP.CurXPLevel);
        UI_Manager.instance.HandlePlayerXPLevel(xpData.TotalXP, xpData.CurXPLevel);

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
