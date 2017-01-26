using System.Collections.Generic;
using UnityEngine;

namespace Script.Manager
{
    //TODO : Incomplete
    public class GameManager : Singleton<GameManager>
    {
        private GameManager() { }

        public static Dictionary<string, int> save;
        public enum State { State1, State2 };
        public enum Flags { bool1, bool2 };

        List<bool> flags;
        public State GameState { get; set; }

        void Start()
        {
            GameState = State.State1;
            flags = new List<bool>(new bool[2]);
        }

        void Add(string vari, int val)
        {
            save.Add(vari, val);
        }

        void ChangeState(Flags trig)
        {
            switch (GameState)
            {
                case State.State1: Debug.Log("1"); break;
                case State.State2: Debug.Log("2"); break;
            }
        }
    }
}