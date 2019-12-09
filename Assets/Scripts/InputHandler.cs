using UnityEngine;

public abstract class InputHandler : MonoBehaviour
{
    [SerializeField]
    private InputManager _inputManager;
	public InputManager InputManager
	{
		get => _inputManager;
		set => _inputManager = value;
	}

	public abstract void HandleInput();

	public virtual void OnSwitchingToThisHandler()
	{
		gameObject.SetActive(true);
	}

	public virtual void OnSwitchingFromThisHandler()
	{
		gameObject.SetActive(false);
	}
}