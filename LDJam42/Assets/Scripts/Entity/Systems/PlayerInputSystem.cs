using UnityEngine;
using System.Collections;
using System;

public class PlayerInputSystem : MonoBehaviour
{
    public static PlayerInputSystem instance { get; protected set; }
    Action<MoveData> OnMoveInput;
    Vector2 lastV2;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
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
