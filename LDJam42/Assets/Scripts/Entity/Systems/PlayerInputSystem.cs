using UnityEngine;
using System.Collections;
using System;
using Global;

public class PlayerInputSystem : MonoBehaviour
{
    public static PlayerInputSystem instance { get; protected set; }
    Action<MoveData> OnMoveInput;
    Vector2 lastV2;
    InputState inputState;

    private void Awake()
    {
        instance = this;

        Global.OnTurnChange.RegisterListener(OnTurnChanged);
    }
    private void OnTurnChanged(OnTurnChange data)
    {
        if (data.newTurnState == TurnState.Player)
        {
            inputState = InputState.Default;
        }
        else
        {
            inputState = InputState.Off;
        }
    }

    private void Update()
    {
        if (inputState == InputState.Off)
            return;

        Vector2 inputV2 = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (inputV2 != lastV2)
        {
            lastV2 = inputV2;
            if (inputV2 == Vector2.zero)
                return;
            if (OnMoveInput != null)
            {
                OnMoveInput(new MoveData(inputV2.x, inputV2.y));
            }
        }
    }

    public void RegisterOnInputCB(Action<MoveData> cb)
    {
        OnMoveInput += cb;
    }
}
public enum InputState
{
    Off, Default
}

public struct MoveData
{
    private readonly float x;
    private readonly float y;

    public MoveData(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public float X
    {
        get
        {
            return x;
        }
    }

    public float Y
    {
        get
        {
            return y;
        }
    }
}
