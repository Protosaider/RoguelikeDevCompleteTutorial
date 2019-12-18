using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class InMenuInputHandler : InputHandler
{
	public EInputHandler _switchToInputHandler;

	public event Action OnKeyDownEscape;

	public override (EInputAction, Object) HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			OnKeyDownEscape?.Invoke();

		return (EInputAction.None, null);
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
