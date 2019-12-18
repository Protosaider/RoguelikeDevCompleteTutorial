using System;
using UnityEngine;

public enum EInputAction
{
	None,
	MoveLeft,
	MoveRight,
	MoveUp,
	MoveDown,
	Wait,
	Escape,
}

public class InGameInputHandler : InputHandler
{
	public override (EInputAction, System.Object) HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			return (EInputAction.Escape, null);

		if (GameManager.CurrentGameState == EGameState.ConcreteGameStatePlayerTurn)
		{
			if (Input.GetKeyDown(KeyCode.D))
				return (EInputAction.MoveRight, EDirection.Right);

			if (Input.GetKeyDown(KeyCode.A))
				return (EInputAction.MoveLeft, EDirection.Left);

			if (Input.GetKeyDown(KeyCode.W))
				return (EInputAction.MoveUp, EDirection.Up);

			if (Input.GetKeyDown(KeyCode.S))
				return (EInputAction.MoveDown, EDirection.Down);

			if (Input.GetKeyDown(KeyCode.Space))
				return (EInputAction.Wait, null);
        }

		return (EInputAction.None, null);
	}

	public override void OnSwitchingToThisHandler()
	{
		//OnKeyDownEscape += SwitchToMainMenuInputHandler;
    }

    public override void OnSwitchingFromThisHandler()
	{
		//OnKeyDownEscape -= SwitchToMainMenuInputHandler;
    }
}