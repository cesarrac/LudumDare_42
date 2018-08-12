using UnityEngine;
using System.Collections;
using System;

public struct EntityState
{
    public StateType stateType;
    public Action action;

    public EntityState(StateType stateType, Action action)
    {
        this.stateType = stateType;
        this.action = action;
    }
}

public enum StateType { Idle, Track, Attack }
