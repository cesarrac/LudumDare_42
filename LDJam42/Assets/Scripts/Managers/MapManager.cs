using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Global;
using UnityEngine;

public class MapManager  {

    public static MapManager instance { get; protected set; }

    int mapWidth = 9, mapHeight = 9;
    
    public GameMap Map { get; protected set; }

    struct TileGOData
    {
        public GameObject mainGO;
        public SpriteRenderer renderer;
    }
    TileGOData[] TileGOs;
    ObjectPool pool;
    GameObject tileHolder;

    int darknessLevel;

    public MapManager()
    {
        instance = this;
        Global.OnTurnChange.RegisterListener(SpreadDarkness);
    }

    
    public void NewMap(Vector2 mapWorldOrigin, int darknessLevel = 0, int mapWidth = 9, int mapHeight = 9)
    {
        this.darknessLevel = darknessLevel;
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
        Map = new GameMap(mapWidth, mapHeight, mapWorldOrigin, OnTileChange);

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

            TileGOs[i] = tileGOData;
        }
        // Clean up array
        TileGOs = TileGOs.Where(go => go.mainGO != null).ToArray();


    }

    void OnTileChange(TileType tileType, int index)
    {
        if (index < 0 || index >= TileGOs.Length)
        {
            Debug.LogError("Trying to change a tile that is not in the GO Tile array at index " + index);
            return;
        }
        // Set Sprite
        RenderSystem.instance.Render(tileType.ToString(), TileGOs[index].renderer);
    }

    public bool CanMoveTo(MoveData newPositionData)
    {
        return !Map.BlocksPath(new Vector2(newPositionData.X, newPositionData.Y));
    }

    private void SpreadDarkness(OnTurnChange data)
    {
        if (data.newTurnState != TurnState.Darkness)
            return;

        int mapwidth = Map.mapWidth;
        int mapHeight = Map.mapHeight;
        //for (int x = 0; x < mapwidth; x++)
        //{
        //    for (int y = 0; y < mapHeight; y++)
        //    {
        //        if(x == darknessLevel)
        //        {
        //            Map.SetTileType(x, y, TileType.Darkness);
        //        }
        //        else if (x == mapwidth - (1 + darknessLevel))
        //        {
        //            Map.SetTileType(x, y, TileType.Darkness);
        //        }
        //        if (y == darknessLevel)
        //        {
        //            Map.SetTileType(x, y, TileType.Darkness);
        //        }
        //        else if(y == mapHeight - (1 + darknessLevel))
        //        {
        //            Map.SetTileType(x, y, TileType.Darkness);
        //        }
        //    }
        //}
        if (darknessLevel > 0)
        {
            int darkCount = 0;
            for (int x = 0; x < mapwidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (darkCount >= darknessLevel)
                        break;
                    int randomChance = UnityEngine.Random.Range(0, 10);
                    if (randomChance == 1)
                    {
                        Map.SetTileType(x, y, TileType.Darkness);
                        darkCount++;
                    }
                }
            }
        }
        darknessLevel++;

        TurnManager.instance.FinishTurn();
    }

}
