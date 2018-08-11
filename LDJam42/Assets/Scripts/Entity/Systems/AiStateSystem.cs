using UnityEngine;
using System.Collections;
using Global;
using System;

public class AiStateSystem : MonoBehaviour
{
    public static AiStateSystem instance { get; protected set; }
    Action DoNextState;
    private void Awake()
    {
        instance = this;
        Global.OnTurnChange.RegisterListener(OnTurnChanged);
    }

    private void OnTurnChanged(OnTurnChange data)
    {
        if (data.newTurnState != TurnState.Enemies)
            return;

        // Begin enemies turn
        if (DoNextState != null)
        {
            Debug.Log("AiState calls DoNextState");
            DoNextState();
        }
    }
    public void RegisterAi(Action cb)
    {
        DoNextState += cb;
    }
}
