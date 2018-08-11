using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager  {

    public static TurnManager instance { get; protected set; }
    TurnState turnState;
    Global.OnTurnChange OnTurnChange;

    public TurnManager(TurnState startTurnState)
    {
        instance = this;
        this.turnState = startTurnState;
        OnTurnChange = new Global.OnTurnChange();
        OnTurnChange.newTurnState = turnState;
        OnTurnChange.FireEvent();
    }
    

    public void FinishTurn()
    {
        int curState = (int)turnState;
        if (curState + 1 >= System.Enum.GetValues(typeof(TurnState)).Length)
        {
            turnState = 0;
            Debug.Log("Starting turn: " + turnState);
            OnTurnChange.newTurnState = turnState;
            OnTurnChange.FireEvent();
            return;
        }
        turnState = (TurnState)curState + 1;
        Debug.Log("Starting turn: " + turnState);
        OnTurnChange.newTurnState = turnState;
        OnTurnChange.FireEvent();
    }
}

public enum TurnState
{
    Player,
    Enemies,
    Darkness
}