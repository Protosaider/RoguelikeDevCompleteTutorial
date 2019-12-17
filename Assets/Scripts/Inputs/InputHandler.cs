using UnityEngine;

public abstract class InputHandler 
{
	protected InputManager InputManager;
	protected GameManager GameManager;

	public virtual void SetManager(GameManager gameManager, InputManager manager)
	{
		GameManager = gameManager;
        InputManager = manager;
	}

    public abstract void HandleInput();
	public abstract void OnSwitchingToThisHandler();
	public abstract void OnSwitchingFromThisHandler();
}