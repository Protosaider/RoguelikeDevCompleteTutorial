using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	//TODO make it singleton

	[SerializeField]
	private GameContext _gameContext;

	public GameState CurrentState => _gameContext.CurrentState;

	public static GameManager s_instance;

	private void Awake()
	{
	}

	private EGameTurn _currentTurn;
	public EGameTurn CurrentTurn
	{
		get => _currentTurn;
		private set => _currentTurn = value;
	}

	public void SwitchGameTurn()
	{
		switch (CurrentTurn)
		{
			case EGameTurn.Player:
				CurrentTurn = EGameTurn.Enemies;
                break;

			case EGameTurn.Enemies:
				CurrentTurn = EGameTurn.Player;
				break;

			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}
