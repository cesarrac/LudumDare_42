using System;
using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour {

    int Level = 0;
    int MaxLevel = 20;

    string[] itemNames;
    LevelPrototype[] levelPrototypes;
    LevelPrototype curLevelPrototype;

    int difficultyLevel = 0;
    bool playerDead = false;

    private void Awake()
    {
        SetLevelPrototypes();
    }

    private void SetLevelPrototypes()
    {
        levelPrototypes = new LevelPrototype[]
        {
            new LevelPrototype("Start", -1, 5, new LevelEnemies[]{}),
            new LevelPrototype("Easy1",1, 0, new LevelEnemies[]{
                                     new LevelEnemies("Feral Frog", 3)}),
            new LevelPrototype("Easy2",1, 0, new LevelEnemies[]{
                                     new LevelEnemies("Feral Frog", 2),
                                     new LevelEnemies("Dark Spider", 1)}),
            new LevelPrototype("Mid1",2, 2, new LevelEnemies[]{
                                     new LevelEnemies("Undead Robot", 1),
                                     new LevelEnemies("Floater", 3)}),
             new LevelPrototype("Mid2",2, 0, new LevelEnemies[]{
                                     new LevelEnemies("Dark Spider", 1),
                                     new LevelEnemies("Floater", 3)}),
              new LevelPrototype("Mid3",3, 0, new LevelEnemies[]{
                                     new LevelEnemies("Dark Spider", 2),
                                     new LevelEnemies("Floater", 1),
                                     new LevelEnemies("Feral Frog", 1)}),
              new LevelPrototype("High",4, 2, new LevelEnemies[]{
                                     new LevelEnemies("Undead Robot", 2),
                                     new LevelEnemies("Dark Spider", 1),
                                     new LevelEnemies("Feral Frog", 1)}),
              new LevelPrototype("High2",5, 3, new LevelEnemies[]{
                                     new LevelEnemies("Dark Spider", 4),
                                     new LevelEnemies("Floater", 1),
                                     new LevelEnemies("Feral Frog", 1)}),
        };
    }
    public void RestartGame()
    {
        // SceneManager.UnloadScene(0);
        //SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
        //SceneManager.LoadScene(0);
        //playerDead = true;
        //EntityManager.instance.ClearPlayer();
        //ClearLevel(true);
        QuitGame();
       
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private void Start()
    {
        itemNames = new string[] { "Iron Sword","Food", "Golden Helm", "Iron Helm", "Iron Spear", "Red Sword", "Red Spear", "Guards Helm", "Poison Helm"};
        StartGame();
    }
    void StartGame ()
    {
        Level = 0;
        difficultyLevel = -1;

        curLevelPrototype = levelPrototypes[0];

        new TurnManager();
        //TurnManager.instance.Init();
        new MapManager();
        
        EntityActionManager.instance.Init();

        Global.PlayerReachedExit.RegisterListener(OnPlayerExit);
        Global.OnMapCleared.RegisterListener(OnMapCleared);
        Global.OnMapCreated.RegisterListener(LoadEntities);
        NewMap();
       
	}
    void ClearLevel(bool restartingGame)
    {
        EntityManager.instance.StopPlayer();
        EntityManager.instance.ClearEnemies();
        EntityManager.instance.ClearItems();
        MapManager.instance.ClearTiles(restartingGame);
    }
    void OnMapCleared(OnMapCleared data)
    {
        //if (playerDead == true)
        //{
        //    playerDead = false;
        //    SceneManager.LoadScene(1);
        //    return;
        //}
        NewMap();
    }

    private void NewMap()
    {

        // Chance to increase difficulty
        if (difficultyLevel <= 0)
        {
            difficultyLevel++;
        }
        else if (UnityEngine.Random.Range(1, 5) == 1)
        {
            difficultyLevel++;
            if (difficultyLevel >= 5)
            {
                // Game win
                Debug.Log("You won the game!");
                Global.GameWin gameWin = new GameWin();
                gameWin.FireEvent();
                return;
            }
        }

        // TODO: PICK PROTOTYPE ACCORDING TO DIFFICULTY!
        if (difficultyLevel > 0)
        {
            List<LevelPrototype> potentialLevels = new List<LevelPrototype>();
            for (int i = 0; i < levelPrototypes.Length; i++)
            {
                if (levelPrototypes[i].difficultyLevel >= difficultyLevel - 1 &&
                    levelPrototypes[i].difficultyLevel <= difficultyLevel)
                {
                    potentialLevels.Add(levelPrototypes[i]);
                    break;
                }
            }
            curLevelPrototype = potentialLevels[UnityEngine.Random.Range(0, potentialLevels.Count)];
        }
        

        if (curLevelPrototype.Name == string.Empty)
        {
            curLevelPrototype = levelPrototypes[1];
        }
        Debug.Log("New Map: diff=" + difficultyLevel + " curlvlProto name=" + curLevelPrototype.Name);

        MapManager.instance.NewMap(Vector2.zero, Level, curLevelPrototype.darknessLevel);

        
    }

    void LoadEntities(OnMapCreated data)
    {
        FXManager.instance.SpawnBackground(Vector2.zero, MapManager.instance.Map.mapWidth, MapManager.instance.Map.mapHeight);
        EntityManager.instance.SpawnPlayer(data.entranceWorldPosition);

        // No enemies on level 0

        if (curLevelPrototype.levelEnemies.Length <= 0)
        {
            // STORY MODE
            MessageLog_Manager.NewMessage("Might be best to go home now before it enters your lungs.", Color.white);
            MessageLog_Manager.NewMessage("But the stench of the Baron lingers in the air still... ", Color.magenta);
            MessageLog_Manager.NewMessage("You vowed to destroy him. Your quest is complete.", Color.white);
            MessageLog_Manager.NewMessage("a necro aristocrat that has dominated your homeland for over 200 years.", Color.white);
            MessageLog_Manager.NewMessage("Before you lies the slaughtered corpse of Baron Rasec", Color.cyan);
            
            

            return;
        }
        for (int i = 0; i < curLevelPrototype.levelEnemies.Length; i++)
        {
            // get a position for enemy needed
            Vector2[] enemyPositions = null;
            enemyPositions = MapManager.instance.GetCleanPositions(curLevelPrototype.levelEnemies[i].count);
            Debug.Log(enemyPositions.Length + " returned positions");
            EntityManager.instance.SpawnEnemies(curLevelPrototype.levelEnemies[i].enemyPrototypeName, enemyPositions);
        }

        int itemCount = UnityEngine.Random.Range(1, 4);
        
        Vector2[] itemPositions = MapManager.instance.GetCleanPositions(itemCount);
        for (int i = 0; i < itemPositions.Length; i++)
        {
            EntityManager.instance.SpawnItem(itemNames[UnityEngine.Random.Range(0, itemNames.Length)], itemPositions[i]);
        }
        
    }

    private void OnPlayerExit(PlayerReachedExit data)
    {
        // Player reached exit.. advance level
        Level++;
        ClearLevel(false);

        TurnManager.instance.Restart();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
