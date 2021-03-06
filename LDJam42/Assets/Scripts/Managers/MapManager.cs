﻿using System;
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
    CameraShaker cameraShaker;

    int darknessLevel;
    struct Darkness
    {
        public int x;
        public int y;
        public float darkValue;

        public Darkness(int x, int y, float darkValue)
        {
            this.x = x;
            this.y = y;
            this.darkValue = darkValue;
        }
    }
    Darkness[] darknessMap;


    public MapManager()
    {
        instance = this;
        Global.OnTurnChange.RegisterListener(SpreadDarkness);
    }

    public void ClearTiles(bool restartingGame)
    {
        for (int i = 0; i < TileGOs.Length; i++)
        {
            TileGOs[i].mainGO.transform.SetParent(null);
            TileGOs[i].mainGO.transform.position = Vector2.zero;
            pool.PoolObject(TileGOs[i].mainGO);

        }
        if (restartingGame == true)
        {
            Map.UnRegisterCB();
            //Map = null;
        }
        Global.OnMapCleared mapCleared = new OnMapCleared();
        mapCleared.FireEvent();
    }
    
    public void NewMap(Vector2 mapWorldOrigin, int level, int darknessLevel = 0, int mapWidth = 9, int mapHeight = 9)
    {
        cameraShaker = CameraShaker.instance;
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
        if (Map != null)
        {
            Map.ResetMap();
        }
        else
        {
            Map = new GameMap(mapWidth, mapHeight, mapWorldOrigin, OnTileChange);
        }
            

        // Make darness map
        darknessMap = new Darkness[mapWidth * mapHeight];
        // Spawn GObjs
        TileGOs = new TileGOData[Map.Tiles.Length];

        Vector2Int exitTilePos = Vector2Int.zero;
        Vector2Int entranceTilePos = Vector2Int.zero;
            
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

            darknessMap[i] = new Darkness(Map.Tiles[i].GridPosition.x, Map.Tiles[i].GridPosition.y, 0);
            

            // Set exit
            if (exitTilePos == Vector2Int.zero)
            {
                if (UnityEngine.Random.Range(1, 50) == 1)
                {
                    exitTilePos = Map.Tiles[i].GridPosition;
                }
                // Set entrance for levels after level 0
                else if (level > 0 && entranceTilePos == Vector2Int.zero)
                {
                    if (UnityEngine.Random.Range(1, 12) == 1)
                    {
                        entranceTilePos = Map.Tiles[i].GridPosition;
                    }
                }
            }

            TileGOs[i] = tileGOData;
            
        }
        // Clean up array
        TileGOs = TileGOs.Where(go => go.mainGO != null).ToArray();

        // if we still have no exit tile, place it in the center of the room
        if (exitTilePos == Vector2Int.zero)
        {
            exitTilePos = new Vector2Int(mapWidth/2, mapHeight/2);
        }
        Map.SetTileType(exitTilePos.x, exitTilePos.y, TileType.Exit);
        if (level > 0)
            Map.SetTileType(entranceTilePos.x, entranceTilePos.y, TileType.Entrance);

        // chance to start with darnkess 
        if (darknessLevel > 0)
        {
            SetDarkness();
        }


        Global.OnMapCreated onMapCreated = new OnMapCreated();
        onMapCreated.entranceWorldPosition = entranceTilePos;
        onMapCreated.exitWorldPosition = exitTilePos;
        onMapCreated.FireEvent();

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

        SetDarkness();

        TurnManager.instance.FinishTurn();
    }

    void SetDarkness()
    {
        int mapwidth = Map.mapWidth;
        int mapHeight = Map.mapHeight;

        bool causedCamShake = false;

        if (darknessLevel > 0)
        {
            int darkCount = 0;
            for (int x = 0; x < mapwidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (darkCount >= darknessLevel)
                        break;
                    if (darknessMap[x + y * mapWidth].darkValue >= 1)
                        continue;

                    int randomChance = UnityEngine.Random.Range(0, 10);
                    if (randomChance == 1)
                    {
                        if (Map.Tiles[x + y * mapWidth].tileType == TileType.Exit ||
                            Map.Tiles[x + y * mapWidth].tileType == TileType.Entrance)
                            continue;
                        // Increase darkness
                        darknessMap[x + y * mapWidth].darkValue += 0.5f;

                        // set darkness if value is at 1 + 
                        if (darknessMap[x + y * mapWidth].darkValue >= 1)
                        {
                            Map.SetTileType(x, y, TileType.Darkness);
                            if (causedCamShake == false)
                            {
                                causedCamShake = true;
                                // cameraShaker.AddTrauma(8, 2);
                            }
                        }
                        else
                        {
                            Map.SetTileType(x, y, TileType.SemiDark);
                        }
                        darkCount++;
                    }
                }
            }
        }
        darknessLevel++;
    }

    public bool ClearDarkTile(Vector2 worlPos)
    {
        int x = Mathf.FloorToInt(worlPos.x);
        int y = Mathf.FloorToInt(worlPos.y);
        MapTile tile = Map.GetTile(worlPos);
        if (tile == null)
            return false;
        if (darknessMap[x + y * Map.mapWidth].darkValue >= 1)
        {
            darknessMap[x + y * Map.mapWidth].darkValue = 0;
            Map.SetTileType(x, y, TileType.Floor);
            return true;
        }
        return false;
    }

    public Vector2[] GetCleanPositions(int totalNeeded)
    {
        Vector2[] positions = new Vector2[totalNeeded];
        int posIndex = 0;
        int maxIterations = 100000, iterations = 0;
        bool allPositionsAcquired = false;
        Vector2 startPos = GetRandomMapPos();
        MapTile curTile = Map.GetTile(startPos);
        //if (curTile == null)
        //{
        //    return null;
        //}
        while(iterations < maxIterations || allPositionsAcquired == false)
        {
            // Set the position to cur tile world position
            if (curTile.tileType == TileType.Floor &&
                    curTile.entities.Count <= 0)
            {
                if (posIndex >= positions.Length)
                {
                    allPositionsAcquired = true;
                    break;
                }
                positions[posIndex] = curTile.WorldPosition;
                posIndex++;
                
                // get new tile
                curTile = Map.GetTile(GetRandomMapPos());
            }
            else
            {
                // get new tile
                curTile = Map.GetTile(GetRandomMapPos());
            }
            iterations++;
            
        }
        Debug.Log("positions " + positions.Length + " found ");
        return positions.Distinct().ToArray();
    }

    Vector2 GetRandomMapPos()
    {
        return new Vector2(UnityEngine.Random.Range(0, Map.mapWidth), UnityEngine.Random.Range(0, Map.mapHeight));
    }
}
