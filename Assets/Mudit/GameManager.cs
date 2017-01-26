using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

	public static Dictionary<string, int> save;

	public enum State { State1, State2 };
	public enum Flags { bool1, bool2};

	List<bool> flags;
	public State GameState;

	// Use this for initialization
	void Start () {
		GameState = State.State1;
		flags = new List<bool>(new bool[2]);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Add(string vari, int val){
		save.Add (vari, val);
	}

	void ChangeState(Flags trig){
		switch (GameState) {
		case State.State1:
			Debug.Log("Y");
		case State.State2:
			Debug.Log("Y");
		}
	}
}
