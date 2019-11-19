using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContext : MonoBehaviour
{
	// A reference to the current state of the Context.
	private GameState _state;

	public GameContext(GameState state) => TransitionTo(state);

	// The Context allows changing the State object at runtime.
	public void TransitionTo(GameState state)
	{
		Debug.Log($"Context: Transition to {state.GetType().Name}.");
		_state = state;
		_state.SetContext(this);
	}

	public void Update()
	{
		Request1();
	}

	// The Context delegates part of its behavior to the current State object.
	public void Request1() //calls current state to react to the request
	{
		_state.Handle1();
	}
}
