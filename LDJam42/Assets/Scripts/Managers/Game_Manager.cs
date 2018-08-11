using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        new MapManager();
        MapManager.instance.NewMap(Vector2.zero);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
