using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityComponent : EntityComponent
{
    Ability[] abilities;

    AbilitySystem abilitySystem;
    PlayerInputSystem playerInputSystem;
    Entity thisEntity;

    public AbilityComponent() : base(ComponentID.Abilities)
    {
        
    }

    public override void Init(Entity entity, GameObject entityGO)
    {
        thisEntity = entity;
        abilitySystem = AbilitySystem.instance;
        playerInputSystem = PlayerInputSystem.instance;
    }

    public void AddAbility(AbilityID abID, bool requiresInput, string description)
    {
        if (abilities == null)
        {
            abilities = new Ability[] { new Ability(abID, requiresInput, description) };

            // add input
            playerInputSystem.AddDynamicKeys(() => abilitySystem.CallAbility(abilities[0], thisEntity), "1");
            UI_Manager.instance.AddButtonAction(() => abilitySystem.CallAbility(abilities[0], thisEntity));
            return;
        }
        if (abilities.Length >= 10)
        {
            // ABILITIES FULL!
            return;
        }
        // Extend array of abilities
        Ability[] origAbilities = abilities;
        abilities = new Ability[origAbilities.Length + 1];
        for (int i = 0; i < origAbilities.Length; i++)
        {
            abilities[i] = origAbilities[i];
        }
        abilities[abilities.Length - 1] = new Ability(abID, requiresInput, description);
        // add input only for new last one added
        string keyName = (abilities.Length).ToString();
        if (abilities.Length >= 10)
            keyName = "0";
        playerInputSystem.AddDynamicKeys(() => abilitySystem.CallAbility(abilities[0], thisEntity), keyName);
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


public struct Ability
{
    private readonly AbilityID id;
    private readonly bool needsInput;
    private readonly string description;

    public Ability(AbilityID name, bool needsInput, string desc)
    {
        this.id = name;
        this.needsInput = needsInput;
        description = desc;
    }

    public AbilityID ID
    {
        get
        {
            return id;
        }
    }

    public bool NeedsInput
    {
        get
        {
            return needsInput;
        }
    }
    public string Name
    {
        get
        {
            string[] ns = id.ToString().Split('_');
            string name = ns[0];
            for (int i = 1; i < ns.Length; i++)
            {
                name += " " + ns[i];
            }
            return name;
        }
    }

    public string Description
    {
        get
        {
            return description;
        }
    }
}