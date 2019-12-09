using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: Make a singleton
/// </summary>
public class GameContext : MonoBehaviour
{
	// A reference to the current state of the Context.
	private GameState _currentState;
	public GameState CurrentState => _currentState;

	public GameContext(GameState state) => TransitionTo(state);

	// The Context allows changing the State object at runtime.
	public void TransitionTo(GameState state)
	{
		Debug.Log($"Context: Transition to {state.GetType().Name}.");
		_currentState = state;
		_currentState.SetContext(this);
	}

	public void Update()
	{
		Request();
	}

	// The Context delegates part of its behavior to the current State object.
	public void Request() //calls current state to react to the request
	{
		_currentState.HandleUpdate();
	}
}
