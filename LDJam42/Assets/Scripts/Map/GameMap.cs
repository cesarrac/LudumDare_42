using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public enum TileType
{
    Empty,
    Floor,
    Wall,
    Darkness,
    Exit,
    SemiDark
}

public class GameMap 
{
    const float DEG2RAD = 3.151459f / 180;

    public int mapWidth, mapHeight;
    public MapTile[] Tiles;

    Vector2 worldOriginPoint;
    Action<TileType, int> OnTileChange;

    public GameMap(int mapWidth, int mapHeight, Vector2 worldOrigin, Action<TileType, int> OnTileChangeCB)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        worldOriginPoint = worldOrigin;
        OnTileChange = OnTileChangeCB;
        InitTiles();
    }
    public void ResetMap()
    {
        InitTiles();
    }

    private void InitTiles()
    {
        Tiles = new MapTile[mapWidth * mapHeight];
        for (int i = 0; i < Tiles.Length; i++)
        {
            int x = i % mapWidth;
            int y = i / mapWidth;

            Tiles[i] = new MapTile(TileType.Floor, new Vector2Int(x, y), new Vector2(worldOriginPoint.x + x, worldOriginPoint.y + y));
        }
    }
    public void SetTileType(int x, int y, TileType type)
    {
        if (IsInMapBounds(x, y) == false) return;
        Tiles[GridIndex(x, y)].tileType = type;
        if (OnTileChange != null)
        {
            OnTileChange(type, GridIndex(x, y));
        }
       // Debug.Log("tile type set to " + type);
    }

    public MapTile GetTile(Vector2 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x);
        int y = Mathf.FloorToInt(worldPos.y);
        return GetTile(x, y);
    }

    public MapTile GetTile(int x, int y)
    {
        if (IsInMapBounds(x, y) == false)
            return null;
        return Tiles[GridIndex(x, y)];
    }
    
    public bool IsSeen(int x, int y)
    {
        if (IsInMapBounds(x, y) == false)
            return false;
        return Tiles[GridIndex(x, y)].seen;
    }
    public bool IsDiscovered(int x, int y)
    {
        if (IsInMapBounds(x, y) == false)
            return false;
        return Tiles[GridIndex(x, y)].discovered;
    }

    public bool IsInMapBounds(int x, int y)
    {
        if (x < 0 || x >= mapWidth) return false;
        if (y < 0 || y >= mapHeight) return false;
        int index = GridIndex(x, y);
        if (index < 0 || index >= Tiles.Length) return false;
        return true;
    }

    public Vector2Int[] GetFOVPositions(int x, int y, int range)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int i = 0; i < 360; i++)
        {
            float deg = i * DEG2RAD;

            int pointX = Mathf.RoundToInt(Mathf.Cos(deg) * range) + x;
            int pointY = Mathf.RoundToInt(Mathf.Sin(deg) * range) + y;

            int diagDist = DiagDist(x, y, pointX, pointY);

            for (int j = 0; j < diagDist; j++)
            {
                int tx = Mathf.FloorToInt(Mathf.Lerp(x, pointX, j / (float)diagDist));
                int ty = Mathf.FloorToInt(Mathf.Lerp(y, pointY, j / (float)diagDist));
                
                if (IsInMapBounds(tx, ty) == false) continue;

                int index = GridIndex(tx, ty);
                // Stop if we reach a wall
                if (Tiles[index].tileType == TileType.Wall)
                {
                    //Tiles[index].seen = true;
                    //Tiles[index].discovered = true;
                    //positions.Add(new Vector2Int(tx, ty));
                    break;
                }
                else if (Tiles[index].tileType == TileType.Empty)
                {
                    break;
                }
                // otherwise mark as seen
                //Tiles[index].seen = true;
                //Tiles[index].discovered = true;
                positions.Add(new Vector2Int(tx, ty));
            }
        }
        if (positions.Count <= 0)
            return null;
        //Debug.Log("Seen position before distinct = " + positions.Count);
        return positions.Distinct().ToArray();
    }

    public void SeeTiles(int x, int y, int range)
    {
        // Clear all tiles
        for (int t = 0; t < Tiles.Length; t++)
        {
            Tiles[t].seen = false;
        }

        for (int i = 0; i < 360; i++)
        {
            float deg = i * DEG2RAD;

            int pointX = Mathf.RoundToInt(Mathf.Cos(deg) * range) + x;
            int pointY = Mathf.RoundToInt(Mathf.Sin(deg) * range) + y;

            int diagDist = DiagDist(x, y, pointX, pointY);

            for (int j = 0; j < diagDist; j++)
            {
                int tx = Mathf.FloorToInt(Mathf.Lerp(x, pointX, j / (float)diagDist));
                int ty = Mathf.FloorToInt(Mathf.Lerp(y, pointY, j / (float)diagDist));

                if (IsInMapBounds(tx, ty) == false) continue;

                int index = GridIndex(tx, ty);
                // Stop if we reach a wall
                if (Tiles[index].tileType == TileType.Wall)
                {
                    Tiles[index].seen = true;
                    Tiles[index].discovered = true;
                    break;
                }
                else if (Tiles[index].tileType == TileType.Empty)
                {
                    break;
                }
                // otherwise mark as seen
                Tiles[index].seen = true;
                Tiles[index].discovered = true;
            }
        }
    }

    static int DiagDist(int x0, int y0, int x1, int y1)
    {
        int dx = x1 - x0, dy = y1 - y0;
        return Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
    }

    public bool BlocksPath(int x, int y)
    {
        if (IsInMapBounds(x, y) == false)
            return true;
        if (Tiles[GridIndex(x, y)].tileType == TileType.Wall)
            return true;
        if (Tiles[GridIndex(x, y)].tileType == TileType.Darkness)
            return true;
        return false;
    }
    public bool BlocksPath(Vector2 position)
    {
        return BlocksPath(Mathf.FloorToInt(position.x), 
                          Mathf.FloorToInt(position.y));
    }

    int GridIndex(int x, int y)
    {
        return x + y * mapWidth;
    }

    public void DebugTile(int x, int y)
    {
        if (IsInMapBounds(x, y) == false)
            Debug.Log("NO Tile to debug!");
        else
        {
            MapTile tile = Tiles[GridIndex(x, y)];
            Debug.Log("Debug tile__ X: " + tile.GridPosition.x + " Y: " + tile.GridPosition.y + " tileType: " + tile.tileType);
        }
    }
}


public class MapTile
{
    public TileType tileType;
    private readonly Vector2Int gridPosition;
    private readonly Vector2 worldPosition;
    public bool seen;
    public bool discovered;

    public List<Entity> entities;

    public Vector2Int GridPosition
    {
        get
        {
            return gridPosition;
        }
    }

    public Vector2 WorldPosition
    {
        get
        {
            return worldPosition;
        }
    }

    public MapTile(TileType tileType, Vector2Int position, Vector2 worldPos)
    {
        this.tileType = tileType;
        this.gridPosition = position;
        this.seen = false;
        this.discovered = false;
        worldPosition = worldPos;
        entities = new List<Entity>();
    }

    public void RegisterEntity(Entity entity)
    {
        entities.Add(entity);
        //Debug.Log("Entity " + entity.Name + " registered to tile");
    }
    public void UnRegisterEntity(Entity entity)
    {
        entities.Remove(entity);
        //Debug.Log("Entity " + entity.Name + " UNregistered to tile");
    }
}


