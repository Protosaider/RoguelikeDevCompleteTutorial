using System;
using UnityEngine;

public class InGameInputHandler : InputHandler
{
    [SerializeField]
    private InputHandler _switchToInputHandler;

	public event Action<EDirection> OnKeyDownD;
	public event Action<EDirection> OnKeyDownA;
	public event Action<EDirection> OnKeyDownW;
	public event Action<EDirection> OnKeyDownS;

	public event Action OnKeyDownEscape;

	public override void HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			OnKeyDownEscape();

		if (Input.GetKeyDown(KeyCode.D))
			OnKeyDownD(EDirection.Right);
		else if (Input.GetKeyDown(KeyCode.A))
			OnKeyDownA(EDirection.Left);
		else if (Input.GetKeyDown(KeyCode.W))
			OnKeyDownW(EDirection.Up);
		else if (Input.GetKeyDown(KeyCode.S))
			OnKeyDownS(EDirection.Down);
	}

	private void OnEnable()
	{
		OnKeyDownEscape += SwitchToMainMenuInputHandler;
		OnKeyDownD += (direction) => Debug.Log("Pressed");
	}

    private void OnDisable()
	{
        OnKeyDownEscape -= SwitchToMainMenuInputHandler;
    }

    private void SwitchToMainMenuInputHandler()
    {
        InputManager.SwitchInputHandlerTo(_switchToInputHandler);
    }

}