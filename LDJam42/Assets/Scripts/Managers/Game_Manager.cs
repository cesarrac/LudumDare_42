using System;
using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;

public class Game_Manager : MonoBehaviour {

    int Level = 0;
    int MaxLevel = 20;

    private void Start()
    {
        StartGame(1);
    }
    void StartGame (int curLevel = 1)
    {
        Level = curLevel;
        new TurnManager(TurnState.Player);
        new MapManager();
        
        EntityActionManager.instance.Init();

        Global.PlayerReachedExit.RegisterListener(OnPlayerExit);
        Global.OnMapCleared.RegisterListener(OnMapCleared);
        Global.OnMapCreated.RegisterListener(LoadLevel);
        NewMap();
       
	}
    void ClearLevel()
    {
        EntityManager.instance.StopPlayer();
        EntityManager.instance.ClearEnemies();
        EntityManager.instance.ClearItems();
        MapManager.instance.ClearTiles();
    }
    void OnMapCleared(OnMapCleared data)
    {
        NewMap();
    }

    private void NewMap()
    {
        MapManager.instance.NewMap(Vector2.zero);
    }

    void LoadLevel(OnMapCreated data)
    {

        EntityManager.instance.SpawnPlayer(data.entranceWorldPosition);

        Vector2 start = Vector2.zero;
        Vector2[] spawnPositions = new Vector2[]
        {
            start + Vector2.up,
            start + Vector2.up * 2,
            start + Vector2.up * 3
        };
        EntityManager.instance.SpawnEnemies(spawnPositions);
    }

    private void OnPlayerExit(PlayerReachedExit data)
    {
        // Player reached exit.. advance level
        Level++;
        if (Level > MaxLevel)
        {
            Global.GameWin gameWin = new GameWin();
            gameWin.FireEvent();
            return;
        }
        ClearLevel();

        TurnManager.instance.Restart();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
