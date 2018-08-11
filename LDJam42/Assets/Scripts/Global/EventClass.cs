using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Global
{
    public abstract class Event<T> where T : Event<T>
    {
        public string Description;
        public delegate void EventListener(T data);
        private static event EventListener listeners;
        public static void RegisterListener(EventListener listener)
        {
            listeners += listener;
        }
        public static void UnRegisterListener(EventListener listener)
        {
            listeners -= listener;
        }
        public void FireEvent()
        {
            if (listeners != null)
            {
                listeners(this as T);
            }
        }
    }
   
    public class EntityGORequestEvent : Event<EntityGORequestEvent>
    {
        public Entity Requester;
    }

    public class MoveToTile : Event<MoveToTile>
    {
        public MoveData lastPositionData;
        public MoveData curPositionData;
        public Entity entity;
    }
    public class OnTurnChange : Event<OnTurnChange>
    {
        public TurnState newTurnState;
    }
    public class PlayerReachedExit : Event<PlayerReachedExit>
    {
        public Vector2 exitPosition;
    }
    public class EntityDeath : Event<EntityDeath>
    {
        public Entity deadEntity;

    }
}
