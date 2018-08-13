using System;
using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;

public class TurnManager {

    public static TurnManager instance { get; protected set; }
    TurnState turnState, storedState;
    Global.OnTurnChange OnTurnChange;
    //float timeToHold = 0.25f;
    //private void Awake()
    //{
    //    instance = this;
        
    //    Global.PlayerReachedExit.RegisterListener(OnPlayerExit);
    //    Global.PlayerDeath.RegisterListener(OnPlayerDeath);
    //}
    public TurnManager()
    {
        instance = this;

        Global.PlayerReachedExit.RegisterListener(OnPlayerExit);
        Global.PlayerDeath.RegisterListener(OnPlayerDeath);
        turnState = TurnState.Player;
        OnTurnChange = new Global.OnTurnChange();
        OnTurnChange.newTurnState = turnState;
        OnTurnChange.FireEvent();
      //  Debug.Log("Starting turn: " + turnState);
    }

    public void Restart()
    {
        turnState = TurnState.Player;
        OnTurnChange.newTurnState = turnState;
        OnTurnChange.FireEvent();
    }

    private void OnPlayerDeath(PlayerDeath data)
    {
        //Debug.Log("OnPlayerDeath");
        turnState = TurnState.NULL;
        OnTurnChange.newTurnState = turnState;
        OnTurnChange.FireEvent();
    }

    private void OnPlayerExit(PlayerReachedExit data)
    {
        Debug.Log("You have reached the EXIT!");
        turnState = TurnState.NULL;
        OnTurnChange.newTurnState = turnState;
        OnTurnChange.FireEvent();
    }

    public void FinishTurn()
    {
        if (turnState == TurnState.NULL)
            return;
        StartNext();
        //StopCoroutine("Hold");
        //storedState = turnState;
        //StartCoroutine("Hold");
        
    }
    void StartNext()
    {
        int curState = (int)turnState;
        if (curState + 1 >= System.Enum.GetValues(typeof(TurnState)).Length)
        {
            turnState = (TurnState)1;
            //MessageLog_Manager.NewMessage("New turn... ", Color.green);
            OnTurnChange.newTurnState = turnState;
            OnTurnChange.FireEvent();
            return;
        }
        turnState = (TurnState)curState + 1;
        
        
        //Debug.Log("Starting turn: " + turnState);
        OnTurnChange.newTurnState = turnState;
        OnTurnChange.FireEvent();
    }
    //IEnumerator Hold()
    //{
    //    turnState = TurnState.NULL;
    //    OnTurnChange.newTurnState = turnState;
    //    OnTurnChange.FireEvent();
    //    yield return new WaitForSeconds(timeToHold);
    //    turnState = storedState;
    //    StartNext();
    //    yield return null;
    //}
}

public enum TurnState
{
    NULL,
    Player,
    Enemies,
    Darkness
}