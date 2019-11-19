using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	//TODO make it singleton

	private void Awake()
	{
		FindObjectOfType<PlayerMovement>().OnMove += SwitchGameTurn;
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
