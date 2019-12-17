using System;
using UnityEngine;

public class InGameInputHandler : InputHandler
{
    private EInputHandler _switchToInputHandler;

	public event Action<EDirection> OnKeyDownD;
	public event Action<EDirection> OnKeyDownA;
	public event Action<EDirection> OnKeyDownW;
	public event Action<EDirection> OnKeyDownS;

	public event Action OnKeyDownEscape;

	public override void HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			OnKeyDownEscape?.Invoke();

		if (GameManager.CurrentGameState == EGameState.ConcreteGameStatePlayerTurn)
		{
			if (Input.GetKeyDown(KeyCode.D))
				OnKeyDownD?.Invoke(EDirection.Right);
			else if (Input.GetKeyDown(KeyCode.A))
				OnKeyDownA?.Invoke(EDirection.Left);
			else if (Input.GetKeyDown(KeyCode.W))
				OnKeyDownW?.Invoke(EDirection.Up);
			else if (Input.GetKeyDown(KeyCode.S))
				OnKeyDownS?.Invoke(EDirection.Down);
        }
	}

	public override void OnSwitchingToThisHandler()
	{
		OnKeyDownEscape += SwitchToMainMenuInputHandler;
    }

    public override void OnSwitchingFromThisHandler()
	{
		OnKeyDownEscape -= SwitchToMainMenuInputHandler;
    }

	private void SwitchToMainMenuInputHandler()
    {
        InputManager.SwitchTo(_switchToInputHandler);
    }

}