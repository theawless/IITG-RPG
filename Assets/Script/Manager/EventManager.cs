using UnityEngine;
using System.Collections.Generic;
using System;

namespace Script.Manager
{
    public class GameEvent
    {
    }

    public class EventManager : Singleton<EventManager>
    {
        public delegate void Delegate<T>(T e) where T : GameEvent;
        private delegate void InternalDelegate(GameEvent e);

        private Dictionary<Type, InternalDelegate> delegates = new Dictionary<Type, InternalDelegate>();
        private Dictionary<Delegate, InternalDelegate> lookups = new Dictionary<Delegate, InternalDelegate>();

        public void AddListener<T>(Delegate<T> del) where T : GameEvent
        {
            if (lookups.ContainsKey(del))
                return;

            // the delegate we actually invoke, it automatically casts the event using lambda expression
            InternalDelegate internalDelLookUp = (e) => del((T)e);
            lookups[del] = internalDelLookUp;

            InternalDelegate internalDel;
            delegates[typeof(T)] = delegates.TryGetValue(typeof(T), out internalDel) ? internalDel += internalDelLookUp : internalDelLookUp;
        }

        public void RemoveListener<T>(Delegate<T> del) where T : GameEvent
        {
            InternalDelegate internalDelLookUp;
            if (lookups.TryGetValue(del, out internalDelLookUp))
            {
                InternalDelegate internalDel;
                if (delegates.TryGetValue(typeof(T), out internalDel))
                {
                    internalDel -= internalDelLookUp;
                    if (internalDel == null)
                        delegates.Remove(typeof(T));
                    else
                        delegates[typeof(T)] = internalDel;
                }
                lookups.Remove(del);
            }
        }

        public bool HasListener<T>(Delegate<T> del) where T : GameEvent
        {
            return lookups.ContainsKey(del);
        }

        public void TriggerEvent(GameEvent e)
        {
            InternalDelegate internalDel;
            if (delegates.TryGetValue(e.GetType(), out internalDel))
                internalDel.Invoke(e);
            else
                Debug.LogWarning("Event: " + e.GetType() + " has no listeners");
        }

        public void RemoveAll()
        {
            delegates.Clear();
            lookups.Clear();
        }

        public void OnApplicationQuit()
        {
            RemoveAll();
        }
    }
}
