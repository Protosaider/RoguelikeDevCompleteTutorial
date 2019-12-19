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
	public MapRenderer _mapRenderer;

    [SerializeField]
	public EntitiesHolder _entitiesHolder;

	[SerializeField]
	public World _world;

	[SerializeField]
	public FieldOfView _fieldOfView;

	public EGameState CurrentGameState => _gameContext.CurrentState;
	public EInputHandler CurrentInputHandler => _inputManager.CurrentHandler;

    private void Awake()
	{
		_gameContext = new GameContext(this, InitialGameState);
		_inputManager = new InputManager(this, InitialInputHandler);

		_mapRenderer = _world.MapRenderer;

        _fieldOfView = new FieldOfView(_world.CurrentMap.Width, _world.CurrentMap.Height);
		_fieldOfView.Recompute(_world.CurrentMap, _entitiesHolder.PlayerEntity.CurrentPosition, _entitiesHolder.PlayerEntity.FovRadius);
    }

	public void Update()
	{
		var inputAction = _inputManager.HandleInput();
		_gameContext.Request(inputAction);
    }
}


