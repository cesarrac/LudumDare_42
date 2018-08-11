using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        new TurnManager(TurnState.Player);
        new MapManager();
        MapManager.instance.NewMap(Vector2.zero);
        EntityActionManager.instance.Init();
        EntityManager.instance.SpawnPlayer(Vector2.right);

        Vector2 start = Vector2.zero;
        Vector2[] spawnPositions = new Vector2[]
        {
            start + Vector2.up,
            start + Vector2.up * 2,
            start + Vector2.up * 3
        };
        EntityManager.instance.SpawnEnemies(spawnPositions);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
