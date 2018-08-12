using UnityEngine;
using System.Collections;
using System;
using Global;
using System.Collections.Generic;

public class PlayerInputSystem : MonoBehaviour
{
    public static PlayerInputSystem instance { get; protected set; }
    Action<MoveData> OnMoveInput;
    Action<KeyCode> OnKeyInput;
    Vector2 lastV2;
    InputState inputState;
    List<DynamicKeyAction> dynamicKeyActions;

    private void Awake()
    {
        instance = this;
        dynamicKeyActions = new List<DynamicKeyAction>();
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
        Vector2 inputV2 = Vector2.zero;
        if (Input.GetButtonDown("DiagTopLeft"))
        {
            inputV2 = Vector2.up + Vector2.left;
        }
        else if (Input.GetButtonDown("DiagTopRight"))
        {
            inputV2 = Vector2.up + Vector2.right;
        }
        else if (Input.GetButtonDown("DiagBottomRight"))
        {
            inputV2 = Vector2.down + Vector2.right;
        }
        else if (Input.GetButtonDown("DiagBottomLeft"))
        {
            inputV2 = Vector2.down + Vector2.left;
        }
        else
        {
            inputV2 = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        

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

        if (dynamicKeyActions.Count > 0)
        {
            for (int i = 0; i < dynamicKeyActions.Count; i++)
            {
                if (Input.GetKeyDown(dynamicKeyActions[i].dynamicKey))
                {
                    dynamicKeyActions[i].action();
                }
            }
        }
    }

    public void RegisterOnMoveInputCB(Action<MoveData> cb)
    {
        OnMoveInput += cb;
    }
    public void UnRegisterOnMoveInputCB(Action<MoveData> cb)
    {
        OnMoveInput += cb;
    }
    public void AddDynamicKeys(Action action, string key)
    {
        
        dynamicKeyActions.Add(new DynamicKeyAction(action, key));
    }
    //public void RegisterKeyInputCB(Action<KeyCode> cb)
    //{
    //    OnKeyInput += cb;
    //}
    //public void UnRegisterKeyInputCB(Action<KeyCode> cb)
    //{
    //    OnKeyInput -= cb;
    //}
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

public class DynamicKeyAction
{
    public Action action;
    public string dynamicKey;

    public DynamicKeyAction(Action action, string dynamicKey)
    {
        this.action = action;
        this.dynamicKey = dynamicKey;
    }
}