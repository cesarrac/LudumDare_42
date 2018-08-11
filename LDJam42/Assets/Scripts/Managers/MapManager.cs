using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager  {

    public static MapManager instance { get; protected set; }

    int mapWidth = 9, mapHeight = 9;
    
    GameMap Map;

    struct TileGOData
    {
        public GameObject mainGO;
        public SpriteRenderer renderer;
    }
    TileGOData[] TileGOs;
    ObjectPool pool;
    GameObject tileHolder;

    public MapManager()
    {
        instance = this;
    }

    public void NewMap(Vector2 mapWorldOrigin, int mapWidth = 9, int mapHeight = 9)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        pool = ObjectPool.instance;
        if (tileHolder == null)
        {
            tileHolder = new GameObject();
            tileHolder.name = "_TILES_";
        }
        tileHolder.transform.position = mapWorldOrigin;

        // Generate the map
        Map = new GameMap(mapWidth, mapHeight, mapWorldOrigin);

        // Spawn GObjs
        TileGOs = new TileGOData[Map.Tiles.Length];
            
        for (int i = 0; i < Map.Tiles.Length; i++)
        {
            if (Map.Tiles[i].tileType == TileType.Empty)
                continue;

            // Spawn tile GO
            GameObject tileGO = pool.GetObjectForType("Tile", true, Map.Tiles[i].WorldPosition);
            if (tileGO == null)
            {
                // Make a new one?
                return;
            }
            tileGO.transform.SetParent(tileHolder.transform);
            TileGOData tileGOData = new TileGOData();
            tileGOData.mainGO = tileGO;
            tileGOData.renderer = tileGO.GetComponentInChildren<SpriteRenderer>();

            // Set Sprite
            RenderSystem.instance.Render(Map.Tiles[i].tileType.ToString(), tileGOData.renderer);
        }
        // Clean up array
        TileGOs = TileGOs.Where(go => go.mainGO != null).ToArray();


    }
}
