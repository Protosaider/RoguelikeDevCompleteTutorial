using UnityEngine;
using Object = System.Object;

public class InputManager : MonoBehaviour
{
	//TODO: Maybe just use ctor from GameManager to setup all stuff?
	[SerializeField]
	private InputHandler _currentHandler;

	public InputManager(InputHandler inputHandler)
	{
		SwitchInputHandlerTo(inputHandler);
	}

	public void SwitchInputHandlerTo(InputHandler inputHandler)
	{
		if (!Object.ReferenceEquals(_currentHandler, null))
			_currentHandler.OnSwitchingFromThisHandler();

		_currentHandler = inputHandler;
		_currentHandler.InputManager = this;

		if (!Object.ReferenceEquals(inputHandler, null))
			_currentHandler.OnSwitchingToThisHandler();
	}

	private void Update()
	{
		if (!Object.ReferenceEquals(_currentHandler, null))
			_currentHandler.HandleInput();
	}

	public void CreateInputHandler<T>() where T : MonoBehaviour
	{
		var gameObject = new GameObject(nameof(T));
		gameObject.AddComponent<T>();
		gameObject.transform.SetParent(transform);
	}
}