using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //TODO make it singleton
	public static GameManager s_instance;

	[SerializeField]
	private EGameState InitialGameState;
    [SerializeField]
	private EInputHandler InitialInputHandler;

    [SerializeField]
	private GameContext _gameContext;
	[SerializeField]
	private InputManager _inputManager;


    [SerializeField]
	public EntitiesHolder _entitiesHolder;

	[SerializeField]
	public World _world;

	public EGameState CurrentGameState => _gameContext.CurrentState;
	public EInputHandler CurrentInputHandler => _inputManager.CurrentHandler;

    private void Awake()
	{
		_gameContext = new GameContext(this, InitialGameState);
		_inputManager = new InputManager(this, InitialInputHandler);
	}

	public void Update()
	{
		var inputAction = _inputManager.HandleInput();
		_gameContext.Request(inputAction);
    }
}


