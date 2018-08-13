using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FXManager : MonoBehaviour
{
    public static FXManager instance { get; protected set; }

    GameObject bgHolder;
    List<GameObject> bgTiles;
    ObjectPool pool;
    RenderSystem renderSystem;
    private void Awake()
    {
        instance = this;
        bgTiles = new List<GameObject>();
    }
    

    public void SpawnBackground(Vector2 mapOriginPos, int mapWidth, int mapHeight)
    {
        if (bgTiles.Count > 0)
            return;

        pool = ObjectPool.instance;
        renderSystem = RenderSystem.instance;
        if (bgHolder == null)
        {
            bgHolder = new GameObject();
            bgHolder.name = "BGHolder";
        }
        GameObject go = null;
        SpriteRenderer renderer;
        for (int x = -1; x < mapWidth + 1; x++)
        {
            for (int y = -1; y < mapHeight + 1; y++)
            {
                Vector2 pos = new Vector2(x, y);
                
                if (x == -1)
                {
                    go = pool.GetObjectForType("BGTile", true, pos);
                    renderer = go.GetComponent<SpriteRenderer>();

                    go.transform.SetParent(bgHolder.transform);
                    if (y == -1)
                    {
                        AddBGTile(go, renderer, "WallLeftCorner");
                    }
                    else
                        AddBGTile(go, renderer, "WallLeft");
                    continue;
                }
                if (x == mapWidth)
                {
                    go = pool.GetObjectForType("BGTile", true, pos);
                    renderer = go.GetComponent<SpriteRenderer>();

                    go.transform.SetParent(bgHolder.transform);
                    if (y == -1)
                    {
                        AddBGTile(go, renderer, "WallRightCorner");
                    }
                    else
                        AddBGTile(go, renderer, "WallRight");
                    continue;
                }
                if (y == -1)
                {
                    go = pool.GetObjectForType("BGTile", true, pos);
                    renderer = go.GetComponent<SpriteRenderer>();
                    AddBGTile(go, renderer, "WallBottom");
                }
                else if (y == mapHeight)
                {
                    go = pool.GetObjectForType("BGTile", true, pos);
                    renderer = go.GetComponent<SpriteRenderer>();

                    AddBGTile(go, renderer, "WallTop");
                }

                
            }
        }
    }
    void AddBGTile(GameObject go, SpriteRenderer renderer, string spriteName)
    {
        go.transform.SetParent(bgHolder.transform);
        renderSystem.Render(spriteName, renderer);
        bgTiles.Add(go);
    }
}
