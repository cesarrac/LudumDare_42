using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderComponent : EntityComponent
{
    public SpriteRenderer renderer { get; protected set; }
    string spriteName;
    public RenderComponent(string spriteName) : base(ComponentID.Render)
    {
        this.spriteName = spriteName;
    }

    public override void Init(Entity entity, GameObject entityGO)
    {
        if (entityGO == null)
            return;
        renderer = entityGO.GetComponentInChildren<SpriteRenderer>();

        RenderSystem.instance.Render(spriteName, renderer);
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
