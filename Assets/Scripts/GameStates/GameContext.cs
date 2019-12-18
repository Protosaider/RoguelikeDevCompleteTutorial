using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// TODO: Make a singleton
/// </summary>
//public class GameContext : MonoBehaviour
[Serializable]
public class GameContext
{
	[NonSerialized]
	private readonly Dictionary<EGameState, GameState> _gameStates;

	// A reference to the current state of the Context.
	[NonSerialized]
	private GameState _currentState;
	[SerializeField]
	private EGameState currentState;
    //public GameState CurrentState => _currentState;
    public EGameState CurrentState => currentState;

    private void SetCurrentState(EGameState enumGameState, GameState gameState)
	{
		currentState = enumGameState;
		_currentState = gameState;
	}

    public GameContext(GameManager gameManager, EGameState initialState)
	{
		var gameStateNames = Enum.GetNames(typeof(EGameState));
        _gameStates = new Dictionary<EGameState, GameState>(gameStateNames.Length);

		foreach (var gameStateName in gameStateNames)
		{
			Enum.TryParse(gameStateName, out EGameState gameStateEnum);

            var gameStateType = Type.GetType(gameStateName);
			if (gameStateType == null)
				Debug.Log("Type not found!");

			_gameStates.Add(
				gameStateEnum,
				(GameState)Activator.CreateInstance(gameStateType)
				);
			_gameStates[gameStateEnum].SetContext(gameManager, this);
        }

		TransitionTo(initialState);
	}

	// The Context allows changing the State object at runtime.
	public void TransitionTo(EGameState state)
	{
        Debug.Log($"{CurrentState.ToString()} wants to change the state of the context.");
        Debug.Log($"Context: Transition to {state.ToString()}.");
		SetCurrentState(state, _gameStates[state]);
	}

	// The Context delegates part of its behavior to the current State object.
	public void Request((EInputAction, System.Object) inputAction) //calls current state to react to the request
	{
		_currentState.HandleUpdate(inputAction);
	}
}