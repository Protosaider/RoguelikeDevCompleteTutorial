using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InMenuInputHandler : InputHandler
{
	public EInputHandler _switchToInputHandler;

	public event Action OnKeyDownEscape;

	public override void HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			OnKeyDownEscape?.Invoke();
	}

	public override void OnSwitchingToThisHandler()
    {
		OnKeyDownEscape += SwitchToGameInputHandler;
	}

	public override void OnSwitchingFromThisHandler()
    {
		OnKeyDownEscape -= SwitchToGameInputHandler;
	}

	private void SwitchToGameInputHandler()
	{
		InputManager.SwitchTo(_switchToInputHandler);
	}
}
