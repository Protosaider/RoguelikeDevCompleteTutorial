using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

[Serializable]
public class InputManager
{
	[NonSerialized]
	private readonly Dictionary<EInputHandler, InputHandler> _inputHandlers;

    [NonSerialized]
	private InputHandler _currentHandler;
	[SerializeField]
	private EInputHandler currentHandler;
	//public InputHandler CurrentHandler => _currentHandler;
	public EInputHandler CurrentHandler => currentHandler;

	private void SetCurrentHandler(EInputHandler enumInputHandler, InputHandler inputHandler)
	{
		currentHandler = enumInputHandler;
		_currentHandler = inputHandler;
	}

	public InputManager(GameManager gameManager, EInputHandler inputHandler)
	{
		var inputHandlerNames = Enum.GetNames(typeof(EInputHandler));
		_inputHandlers = new Dictionary<EInputHandler, InputHandler>(inputHandlerNames.Length);

		foreach (var inputHandlerName in inputHandlerNames)
		{
			Enum.TryParse(inputHandlerName, out EInputHandler gameStateEnum);

			var inputHandlerType = Type.GetType(inputHandlerName);
			if (inputHandlerType == null)
				Debug.Log("Type not found!");

			_inputHandlers.Add(
				gameStateEnum,
				(InputHandler)Activator.CreateInstance(inputHandlerType)
			);
			_inputHandlers[gameStateEnum].SetManager(gameManager, this);
		}

        SwitchTo(inputHandler);
	}

	public void SwitchTo(EInputHandler inputHandler)
	{
		if (!Object.ReferenceEquals(_currentHandler, null))
			_currentHandler.OnSwitchingFromThisHandler();

		SetCurrentHandler(inputHandler, _inputHandlers[inputHandler]);

		if (!Object.ReferenceEquals(_currentHandler, null))
			_currentHandler.OnSwitchingToThisHandler();
	}

	public (EInputAction, System.Object) HandleInput()
	{
		if (!Object.ReferenceEquals(_currentHandler, null))
			return _currentHandler.HandleInput();

		return (EInputAction.None, null);
	}

}