using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InMenuInputHandler : InputHandler
{
	[SerializeField]
	private InputHandler _switchToInputHandler;

	public event Action OnKeyDownEscape;

	public override void HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			OnKeyDownEscape();
	}

	private void OnEnable()
	{
		OnKeyDownEscape += SwitchToGameInputHandler;
	}

	private void OnDisable()
	{
		OnKeyDownEscape -= SwitchToGameInputHandler;
	}

	private void SwitchToGameInputHandler()
	{
		InputManager.SwitchInputHandlerTo(_switchToInputHandler);
	}
}
