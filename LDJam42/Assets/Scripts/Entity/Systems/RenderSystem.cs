using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderSystem : MonoBehaviour {
    public static RenderSystem instance { get; protected set; }
    Dictionary<string, Sprite> spriteMap;

    private void Awake()
    {
        instance = this;
        spriteMap = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/");
        for (int i = 0; i < sprites.Length; i++)
        {
            spriteMap.Add(sprites[i].name, sprites[i]);
        }
    }

    public void Render(string spriteName, SpriteRenderer renderer)
    {
        if (spriteMap.ContainsKey(spriteName) == false)
            return;
        renderer.sprite = spriteMap[spriteName];
    }
}
