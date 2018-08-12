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
            //Debug.Log("AiState calls DoNextState");
            DoNextState();
        }
        else
        {
            // no enemies left, end enemy turn
            TurnManager.instance.FinishTurn();
        }
    }
    public void RegisterAi(Action cb)
    {
        DoNextState += cb;
    }
    public void UnRegisterAi(Action cb)
    {
        DoNextState -= cb;
        //Debug.Log("Ai unregistered");
    }

    public MoveData GetDirectionToTarget(MoveData enemyMoveData, MoveData playerPosData)
    {
        float x = 0;
        float y = 0;
        if (playerPosData.X < enemyMoveData.X)
        {
            x = -1;
        }
        else if (playerPosData.X > enemyMoveData.X)
        {
            x = 1;
        }
        if (playerPosData.Y < enemyMoveData.Y)
        {
            y = -1;
        }
        else if (playerPosData.Y > enemyMoveData.Y)
        {
            y = 1;
        }
        Vector2 nextDirection = new Vector2(x, y);
        Vector2 nextPosition = new Vector2(enemyMoveData.X + x, enemyMoveData.Y + y);
        MapTile nextTile = MapManager.instance.Map.GetTile(nextPosition);

        if (nextTile == null)
        {
            return new MoveData(0, 0);
        }

        if (nextTile.entities.Count > 0)
        {
            bool playerFound = false;
            int maxIterations = 2, iterations = 0;
            while (iterations < maxIterations)
            {
                for (int i = 0; i < nextTile.entities.Count; i++)
                {
                    if (nextTile.entities[i].isPlayer == true)
                    {
                        playerFound = true;
                        break;
                    }
                }
                if (playerFound == true)
                    break;
                float nextX = 0;
                float nextY = 0;
                if (nextDirection.x > 1 || nextDirection.x < 1)
                {
                    if (nextDirection.y == 0)
                        nextX = nextDirection.x;

                    if (playerPosData.Y > enemyMoveData.Y)
                        nextY = 1;
                    else
                        nextY = -1;
                }
                else if (nextDirection.y > 1 || nextDirection.y < 1)
                {
                    if (playerPosData.X > enemyMoveData.X)
                        nextX = 1;
                    else
                        nextX = -1;

                    if (nextDirection.x == 0)
                        nextY = nextDirection.y;
                }
                nextDirection = new Vector2(nextX, nextY);
                nextPosition = new Vector2(enemyMoveData.X + nextX, enemyMoveData.Y + nextY);
                // need a new position
                nextTile = MapManager.instance.Map.GetTile(nextPosition);
                if (nextTile == null || iterations + 1 >= maxIterations)
                {
                    nextDirection = Vector2.zero;
                    break;
                }
                iterations++;
            }
            
        }

        return new MoveData(nextDirection.x, nextDirection.y);
    }
}
